from pypdf import PdfReader, PdfWriter

# Read the PDF
reader = PdfReader("Example_46.pdf")
writer = PdfWriter()

# Copy all pages
for page in reader.pages:
    writer.add_page(page)

# Encrypt with AES-256 v5 r6
writer.encrypt(
    user_password="hello",
    algorithm="AES-256"
)

# Save encrypted PDF
with open("Example_46_encrypted.pdf", "wb") as f:
    writer.write(f)

print("PDF encrypted with AES-256 v5 r6 successfully!")
