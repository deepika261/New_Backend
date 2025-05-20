import cv2
import numpy as np
import io
import sys
import os
import unicodedata
from google.cloud import vision
from google.cloud.vision_v1 import types

#  Fix for Unicode output issues
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# Set your Google API credentials
os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "D:/Users/daffny/Desktop/Deepika/handwrittentextconverter-94906516dcd3.json"

def detect_text(image_path):
    client = vision.ImageAnnotatorClient()

    with io.open(image_path, 'rb') as image_file:
        content = image_file.read()

    image = vision.Image(content=content)
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

def clean_text(text):
    return ''.join(
        c for c in unicodedata.normalize("NFKD", text)
        if not unicodedata.category(c).startswith("C")
    )

if __name__ == "__main__":
    if len(sys.argv) < 2:
        sys.exit(1)

    image_path = sys.argv[1]

    try:
        raw_text = detect_text(image_path)
        cleaned_ocr = clean_text(raw_text)
        reconstructed_text = remove_letter_spacing(cleaned_ocr)

        print(reconstructed_text)

    except Exception as e:
        print(f"Error: {str(e)}")
