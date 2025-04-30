import cv2
import numpy as np
import io
import os
from google.cloud import vision
from google.cloud.vision_v1 import types

# Set environment variable to your API key file
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "D:/Users/daffny/Desktop/Deepika/handwrittentextconverter-94906516dcd3.json"

def preprocess_image(image_path: str) -> bytes:
    img = cv2.imread(image_path)
    if img is None:
        raise Exception("Image not loaded. Check the path.")

    img = cv2.resize(img, None, fx=1.5, fy=1.5, interpolation=cv2.INTER_LINEAR)
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    gray = cv2.GaussianBlur(gray, (5, 5), 0)

    bin_img = cv2.adaptiveThreshold(
        gray, 255, cv2.ADAPTIVE_THRESH_MEAN_C,
        cv2.THRESH_BINARY, 11, 2
    

    )
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (2, 2))
    bin_img = cv2.morphologyEx(bin_img, cv2.MORPH_CLOSE, kernel)
    coords = np.column_stack(np.where(bin_img > 0))
    angle = cv2.minAreaRect(coords)[-1]
    angle = -(90 + angle) if angle < -45 else -angle
    (h, w) = bin_img.shape[:2]
    M = cv2.getRotationMatrix2D((w // 2, h // 2), angle, 1.0)
    deskewed = cv2.warpAffine(bin_img, M, (w, h),
                              flags=cv2.INTER_CUBIC, borderMode=cv2.BORDER_REPLICATE)

    # Encode to bytes
    _, encoded_image = cv2.imencode('.png', deskewed)
    return encoded_image.tobytes()

def detect_text_bytes(image_bytes):
    client = vision.ImageAnnotatorClient()
    image = vision.Image(content=image_bytes)
    response = client.text_detection(image=image)
    texts = response.text_annotations

    if texts:
        return texts[0].description
    else:
        return "No text detected."

def remove_letter_spacing(text):
    words = text.split()
    joined_text = ""
    temp_word = ""

    for word in words:
        if len(word) == 1:  # likely a letter in spaced handwriting
            temp_word += word
        else:
            if temp_word:
                joined_text += temp_word + " "
                temp_word = ""
            joined_text += word + " "

    if temp_word:
        joined_text += temp_word

    return joined_text.strip()

image_path = "D:/Users/daffny/Desktop/Deepika/OCRSolution/Uploads/image1.jpg"

# Step 1: Preprocess and get image bytes
preprocessed_image_bytes = preprocess_image(image_path)

# Step 2: Run OCR on the preprocessed image
raw_text = detect_text_bytes(preprocessed_image_bytes)
print("\n Raw OCR Output:\n", raw_text)

# Step 3: Remove weird spacing between letters
paragraph = remove_letter_spacing(raw_text)
print("\n Reconstructed Paragraph:\n", paragraph)

