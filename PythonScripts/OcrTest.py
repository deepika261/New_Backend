import sys
import pytesseract
from PIL import Image

def extract_text(image_path):
    try:
        text = pytesseract.image_to_string(Image.open(image_path))
        print(text)
    except Exception as e:
        print(f"Error: {str(e)}")

if __name__ == "__main__":
    extract_text(D:\Users\daffny\Desktop\Deepika\OCRSolution\Uploads\image1.jpg)
