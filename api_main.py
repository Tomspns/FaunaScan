import os
import logging
from datetime import datetime, timedelta
from typing import Optional

import httpx
from fastapi import FastAPI, UploadFile, File, HTTPException, Depends, status
from fastapi.security import OAuth2PasswordBearer, OAuth2PasswordRequestForm
from jose import JWTError, jwt
from passlib.context import CryptContext
from motor.motor_asyncio import AsyncIOMotorClient
from pydantic import BaseModel

# ─────────────────────────────────────
# Configuration
# ─────────────────────────────────────
MONGO_URL         = os.getenv("MONGO_URL",         "mongodb://mongodb:27017")
WIKIPEDIA_URL     = os.getenv("WIKIPEDIA_URL",     "http://wikipedia:8001")
YOLO_URL          = os.getenv("YOLO_URL",          "http://yolov8:8002")
JWT_SECRET        = os.getenv("JWT_SECRET",        "change_this_secret_in_production")
JWT_ALGORITHM     = os.getenv("JWT_ALGORITHM",     "HS256")
JWT_EXPIRE_MINUTES= int(os.getenv("JWT_EXPIRE_MINUTES", "60"))

app    = FastAPI(title="Animal Detection API")
logger = logging.getLogger(__name__)

# ─────────────────────────────────────
# MongoDB
# ─────────────────────────────────────
mongo_client      = AsyncIOMotorClient(MONGO_URL)
db                = mongo_client["animal_detection"]
users_collection  = db["users"]
detect_collection = db["detections"]

# ─────────────────────────────────────
# JWT & Sécurité
# ─────────────────────────────────────
pwd_context    = CryptContext(schemes=["bcrypt"], deprecated="auto")
oauth2_scheme  = OAuth2PasswordBearer(tokenUrl="/auth/login")


# ─────────────────────────────────────
# Modèles Pydantic
# ─────────────────────────────────────
class UserCreate(BaseModel):
    username : str
    password : str

class Token(BaseModel):
    access_token : str
    token_type   : str


# ─────────────────────────────────────
# Fonctions JWT
# ─────────────────────────────────────
def hash_password(password: str) -> str:
    return pwd_context.hash(password)

def verify_password(plain: str, hashed: str) -> bool:
    return pwd_context.verify(plain, hashed)

def create_token(data: dict) -> str:
    payload = data.copy()
    payload["exp"] = datetime.utcnow() + timedelta(minutes=JWT_EXPIRE_MINUTES)
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

async def get_current_user(token: str = Depends(oauth2_scheme)):
    try:
        payload  = jwt.decode(token, JWT_SECRET, algorithms=[JWT_ALGORITHM])
        username = payload.get("sub")
        if not username:
            raise HTTPException(status_code=401, detail="Token invalide")
        user = await users_collection.find_one({"username": username})
        if not user:
            raise HTTPException(status_code=401, detail="Utilisateur introuvable")
        return user
    except JWTError:
        raise HTTPException(status_code=401, detail="Token invalide ou expiré")


# ═════════════════════════════════════
# ROUTES AUTHENTIFICATION
# ═════════════════════════════════════
@app.get("/health")
async def health():
    return {"status": "ok", "service": "api"}


@app.post("/auth/register", status_code=201)
async def register(user: UserCreate):
    """Créer un nouvel utilisateur"""
    existing = await users_collection.find_one({"username": user.username})
    if existing:
        raise HTTPException(status_code=400, detail="Nom d'utilisateur déjà pris")

    await users_collection.insert_one({
        "username"  : user.username,
        "password"  : hash_password(user.password),
        "created_at": datetime.utcnow()
    })
    return {"message": "Utilisateur créé ✅"}


@app.post("/auth/login", response_model=Token)
async def login(form: OAuth2PasswordRequestForm = Depends()):
    """Connexion et récupération du token JWT"""
    user = await users_collection.find_one({"username": form.username})
    if not user or not verify_password(form.password, user["password"]):
        raise HTTPException(status_code=401, detail="Identifiants incorrects")

    token = create_token({"sub": user["username"]})
    return {"access_token": token, "token_type": "bearer"}


# ═════════════════════════════════════
# ROUTE DÉTECTION (protégée par JWT)
# ═════════════════════════════════════
@app.post("/detect")
async def detect(
    image: UploadFile = File(...),
    current_user = Depends(get_current_user)
):
    """
    Détecte les animaux dans une image.
    Orchestre YOLOv8 → Wikipedia → MongoDB.
    Nécessite un token JWT valide.
    """
    image_bytes = await image.read()
    resultats   = []

    async with httpx.AsyncClient(timeout=30.0) as client:

        # ──────────────────────────────
        # ÉTAPE 1 — YOLOv8 : détection
        # ──────────────────────────────
        try:
            yolo_response = await client.post(
                f"{YOLO_URL}/predict",
                files={"image": (image.filename, image_bytes, image.content_type)}
            )
            yolo_data  = yolo_response.json()
            detections = yolo_data.get("detections", [])
        except Exception as e:
            raise HTTPException(status_code=503, detail=f"Service YOLOv8 indisponible : {str(e)}")

        # ──────────────────────────────
        # ÉTAPE 2 — Wikipedia : enrichissement
        # ──────────────────────────────
        for detection in detections:
            nom_animal = detection["animal"]
            wiki_data  = None

            try:
                wiki_response = await client.get(
                    f"{WIKIPEDIA_URL}/animal/{nom_animal}"
                )
                if wiki_response.status_code == 200:
                    wiki_data = wiki_response.json()
            except Exception as e:
                logger.warning(f"Wikipedia indisponible pour {nom_animal} : {e}")

            resultats.append({
                "animal"    : nom_animal,
                "confiance" : detection["confiance"],
                "bbox"      : detection["bbox"],
                "wikipedia" : wiki_data
            })

    # ──────────────────────────────
    # ÉTAPE 3 — MongoDB : sauvegarde
    # ──────────────────────────────
    document = {
        "date"          : datetime.utcnow(),
        "utilisateur"   : current_user["username"],
        "image_nom"     : image.filename,
        "nb_detections" : len(resultats),
        "detections"    : resultats
    }
    inserted = await detect_collection.insert_one(document)

    return {
        "nb_detections": len(resultats),
        "detections"   : resultats,
        "sauvegarde_id": str(inserted.inserted_id)
    }


# ═════════════════════════════════════
# ROUTE HISTORIQUE (protégée par JWT)
# ═════════════════════════════════════
@app.get("/historique")
async def get_historique(
    limite: int = 10,
    current_user = Depends(get_current_user)
):
    """Retourne les dernières détections de l'utilisateur connecté"""
    detections = await detect_collection.find(
        {"utilisateur": current_user["username"]},
        {"_id": 0}
    ).sort("date", -1).limit(limite).to_list(limite)

    return {"detections": detections}


# ═════════════════════════════════════
# ROUTE STATS (protégée par JWT)
# ═════════════════════════════════════
@app.get("/stats")
async def get_stats(current_user = Depends(get_current_user)):
    """Retourne les statistiques de détection de l'utilisateur connecté"""
    pipeline = [
        {"$match"  : {"utilisateur": current_user["username"]}},
        {"$unwind" : "$detections"},
        {"$group"  : {
            "_id"           : "$detections.animal",
            "total"         : {"$sum": 1},
            "moy_confiance" : {"$avg": "$detections.confiance"}
        }},
        {"$sort"   : {"total": -1}},
        {"$project": {
            "_id"          : 0,
            "animal"       : "$_id",
            "total"        : 1,
            "moy_confiance": {"$round": ["$moy_confiance", 2]}
        }}
    ]

    stats = await detect_collection.aggregate(pipeline).to_list(100)
    return {"stats": stats}
