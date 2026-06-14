import cv2
import os
from ultralytics import YOLO

# ==================================
# CONFIGURACIÓN
# ==================================
INPUT_PATH = "../entrada/entrada.jpeg"  # Cambiar por imagen o video
OUTPUT_DIR = "../resultados"

os.makedirs(OUTPUT_DIR, exist_ok=True)

# ==================================
# CARGAR MODELO YOLO
# ==================================
model = YOLO("yolov8n.pt")

# ==================================
# FUNCIONES
# ==================================

def procesar_imagen(image_path):
    print(f"Procesando imagen: {image_path}")

    img = cv2.imread(image_path)

    if img is None:
        print("No se pudo cargar la imagen")
        return

    # --------------------------
    # Escala de grises
    # --------------------------
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    # --------------------------
    # HSV
    # --------------------------
    hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)

    # --------------------------
    # LAB
    # --------------------------
    lab = cv2.cvtColor(img, cv2.COLOR_BGR2LAB)

    # --------------------------
    # Bordes
    # --------------------------
    edges = cv2.Canny(gray, 100, 200)

    # --------------------------
    # YOLO
    # --------------------------
    detected = img.copy()

    results = model(img)

    for result in results:
        for box in result.boxes:

            x1, y1, x2, y2 = map(int, box.xyxy[0])

            confidence = float(box.conf[0])

            class_id = int(box.cls[0])

            class_name = model.names[class_id]

            label = f"{class_name} {confidence:.2f}"

            cv2.rectangle(
                detected,
                (x1, y1),
                (x2, y2),
                (0, 255, 0),
                2
            )

            cv2.putText(
                detected,
                label,
                (x1, y1 - 10),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.5,
                (0, 255, 0),
                2
            )

    # --------------------------
    # Guardar resultados
    # --------------------------
    cv2.imwrite(f"{OUTPUT_DIR}/01_original.jpg", img)
    cv2.imwrite(f"{OUTPUT_DIR}/02_grises.jpg", gray)
    cv2.imwrite(f"{OUTPUT_DIR}/03_hsv.jpg", hsv)
    cv2.imwrite(f"{OUTPUT_DIR}/04_lab.jpg", lab)
    cv2.imwrite(f"{OUTPUT_DIR}/05_bordes.jpg", edges)
    cv2.imwrite(f"{OUTPUT_DIR}/06_deteccion.jpg", detected)

    print("Resultados guardados correctamente.")


def procesar_video(video_path):
    print(f"Procesando video: {video_path}")

    cap = cv2.VideoCapture(video_path)

    if not cap.isOpened():
        print("No se pudo abrir el video")
        return

    fps = cap.get(cv2.CAP_PROP_FPS)

    width = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
    height = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))

    fourcc = cv2.VideoWriter_fourcc(*'mp4v')

    writers = {
        "original": cv2.VideoWriter(
            f"{OUTPUT_DIR}/01_original.mp4",
            fourcc,
            fps,
            (width, height)
        ),

        "grises": cv2.VideoWriter(
            f"{OUTPUT_DIR}/02_grises.mp4",
            fourcc,
            fps,
            (width, height)
        ),

        "hsv": cv2.VideoWriter(
            f"{OUTPUT_DIR}/03_hsv.mp4",
            fourcc,
            fps,
            (width, height)
        ),

        "lab": cv2.VideoWriter(
            f"{OUTPUT_DIR}/04_lab.mp4",
            fourcc,
            fps,
            (width, height)
        ),

        "bordes": cv2.VideoWriter(
            f"{OUTPUT_DIR}/05_bordes.mp4",
            fourcc,
            fps,
            (width, height)
        ),

        "deteccion": cv2.VideoWriter(
            f"{OUTPUT_DIR}/06_deteccion.mp4",
            fourcc,
            fps,
            (width, height)
        )
    }

    while True:

        ret, frame = cap.read()

        if not ret:
            break

        # --------------------------
        # Grises
        # --------------------------
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        gray_bgr = cv2.cvtColor(gray, cv2.COLOR_GRAY2BGR)

        # --------------------------
        # HSV
        # --------------------------
        hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
        hsv_bgr = cv2.cvtColor(hsv, cv2.COLOR_HSV2BGR)

        # --------------------------
        # LAB
        # --------------------------
        lab = cv2.cvtColor(frame, cv2.COLOR_BGR2LAB)
        lab_bgr = cv2.cvtColor(lab, cv2.COLOR_LAB2BGR)

        # --------------------------
        # Bordes
        # --------------------------
        edges = cv2.Canny(gray, 100, 200)
        edges_bgr = cv2.cvtColor(edges, cv2.COLOR_GRAY2BGR)

        # --------------------------
        # Detección YOLO
        # --------------------------
        detected = frame.copy()

        results = model(frame)

        for result in results:
            for box in result.boxes:

                x1, y1, x2, y2 = map(int, box.xyxy[0])

                confidence = float(box.conf[0])

                class_id = int(box.cls[0])

                class_name = model.names[class_id]

                label = f"{class_name} {confidence:.2f}"

                cv2.rectangle(
                    detected,
                    (x1, y1),
                    (x2, y2),
                    (0, 255, 0),
                    2
                )

                cv2.putText(
                    detected,
                    label,
                    (x1, y1 - 10),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    0.5,
                    (0, 255, 0),
                    2
                )

        # --------------------------
        # Guardar frame en videos
        # --------------------------
        writers["original"].write(frame)
        writers["grises"].write(gray_bgr)
        writers["hsv"].write(hsv_bgr)
        writers["lab"].write(lab_bgr)
        writers["bordes"].write(edges_bgr)
        writers["deteccion"].write(detected)

    cap.release()

    for writer in writers.values():
        writer.release()

    print("Videos procesados correctamente.")


# ==================================
# EJECUCIÓN
# ==================================

ext = os.path.splitext(INPUT_PATH)[1].lower()

if ext in [".jpg", ".jpeg", ".png", ".bmp"]:
    procesar_imagen(INPUT_PATH)

elif ext in [".mp4", ".avi", ".mov", ".mkv"]:
    procesar_video(INPUT_PATH)

else:
    print("Formato no soportado")