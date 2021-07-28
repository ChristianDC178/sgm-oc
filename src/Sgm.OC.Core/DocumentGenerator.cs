using System;
using System.Collections.Generic;
using System.Text;
using Sgm.OC.Domain.Entities;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;

namespace Sgm.OC.Core
{
    public class DocumentGenerator
    {

        public byte[] CreateRequisicionDocument(Requisicion requisicion)
        {

            using (MemoryStream memoryStream = new MemoryStream())
            {

                string imagePath = System.IO.Path.Combine(Environment.CurrentDirectory, "logo.png");
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(memoryStream));
                Document document = new Document(pdfDocument);

                string dateParagraph = $"Fecha {DateTime.Now.ToString("dd/MM/yyyy")}";
                string titleParagraph = $"Pedido de Cotizacion - Requisicion nro {requisicion.Id}";

                string msgParagraph = $"Se solicita la cotización para los siguientes productos";

                Table table = new Table(4);
                Style style = new Style();

                style.SetFontSize(10);

                table.AddStyle(style);

                table.AddHeaderCell("Linea");
                table.AddHeaderCell("Descripcion");
                table.AddHeaderCell("Unidad de Medida");
                table.AddHeaderCell("Cantidad");

                int numLinea = 0;

                foreach (var item in requisicion.Items)
                {
                    numLinea++;
                    table.AddCell(numLinea.ToString());
                    table.AddCell(item.Producto.Descripcion);
                    table.AddCell(item.EnUnidades ? "Unidades" : "Bultos");
                    table.AddCell(item.Cantidad.ToString());
                }

                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(imagePath));
                Image image = new Image(xObject, 100);

                Paragraph imageParagraph = new Paragraph();
                imageParagraph.Add(image);

                document.Add(imageParagraph);
                document.Add(new Paragraph(dateParagraph));
                document.Add(new Paragraph(titleParagraph));
                document.Add(new Paragraph());
                document.Add(new Paragraph());
                document.Add(new Paragraph(msgParagraph));
                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }

        }



    }
}
