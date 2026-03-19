import logging
import os
from fastapi import FastAPI, UploadFile, File, HTTPException
from PIL import Image
import io

app    = FastAPI(title="YOLOv8 Detection Service")
logger = logging.getLogger(__name__)

WEIGHTS_PATH = os.getenv("WEIGHTS_PATH", "/app/weights/best.pt")

# ─────────────────────────────────────
# Chargement du modèle YOLOv8
# Le modèle sera chargé au démarrage si best.pt est présent
# ─────────────────────────────────────
model = None

def charger_modele():
    global model
    if os.path.exists(WEIGHTS_PATH):
        from ultralytics import YOLO
        model = YOLO(WEIGHTS_PATH)
        logger.info(f"Modèle YOLOv8 chargé depuis {WEIGHTS_PATH} ✅")
    else:
        logger.warning(f"Modèle introuvable : {WEIGHTS_PATH} — service en attente du modèle")

charger_modele()


# ─────────────────────────────────────
# Endpoints
# ─────────────────────────────────────
@app.get("/health")
async def health():
    return {
        "status" : "ok",
        "service": "yolov8",
        "modele" : "chargé ✅" if model else "en attente de best.pt ⏳"
    }


@app.post("/predict")
async def predict(image: UploadFile = File(...)):
    """
    Reçoit une image et retourne les détections YOLOv8.
    Appelé par le service FastAPI principal.
    """

    # Vérifier que le modèle est chargé
    if model is None:
        raise HTTPException(
            status_code=503,
            detail="Modèle YOLOv8 non disponible. Déposez best.pt dans yolov8/weights/"
        )

    # Lire et préparer l'image
    image_bytes = await image.read()
    img         = Image.open(io.BytesIO(image_bytes)).convert("RGB")

    # Lancer la détection
    results     = model.predict(source=img, conf=0.5, verbose=False)
    detections  = []

    for result in results:
        for box in result.boxes:
            classe_id  = int(box.cls[0])
            nom_animal = model.names[classe_id]
            confiance  = round(float(box.conf[0]), 4)
            bbox       = [round(float(x), 2) for x in box.xyxy[0].tolist()]

            detections.append({
                "animal"   : nom_animal,
                "confiance": confiance,
                "bbox"     : bbox
            })

    return {
        "nb_detections": len(detections),
        "detections"   : detections
    }


@app.post("/reload")
async def reload_model():
    """
    Recharge le modèle sans redémarrer le conteneur.
    Utile après avoir déposé un nouveau best.pt.
    """
    charger_modele()
    return {
        "status": "rechargé ✅" if model else "best.pt toujours introuvable ⚠️"
    }
