using System;
using System.IO;
using System.Threading;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;
using Sgm.OC.Domain.Entities;
using System.Collections.Generic;

namespace Sgm.OC.Pdf
{

    class Program
    {

        static void Main()
        {

            Requisicion requisicion = new Requisicion(1, 1);
            requisicion.Id = 5;

            List<RequisicionItem> items = new List<RequisicionItem>()
            {
                 new RequisicionItem(1,2 ,  false) {  Producto = new Producto() { Descripcion = "Producto 1" } },
                 new RequisicionItem(2,50, true) {  Producto = new Producto() { Descripcion = "Producto 2sssssssssssssssss" } },
                 new RequisicionItem(3,100, true) {  Producto = new Producto() { Descripcion = "Producto 3" }},
                 new RequisicionItem(4,150, false) {  Producto = new Producto() { Descripcion = "Producto 4" }}
            };

            Proveedor proveedor = new Proveedor()
            {
                Descripcion = "Distribuidor S.R.L"
            };

            requisicion.Items = items;

            //https://github.com/itext/itext7-dotnet.git
            string imagePath = System.IO.Path.Combine(Environment.CurrentDirectory, "logo.png");
            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, "diarco1.pdf");

            //PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(System.IO.Path.Combine(Environment.CurrentDirectory, "diarco.pdf"), FileMode.Create, FileAccess.Write)));
            using (MemoryStream memoryStream = new MemoryStream())
            {

                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(memoryStream));
                Document document = new Document(pdfDocument);

                string dateLine = $"Fecha {DateTime.Now.ToString("dd/MM/yyyy")}";
                string titleLine = $"Pedido de Cotización - Requisición {requisicion.Id}";

                string supplierLine = $"Estimado {proveedor.Descripcion}, solicitamos la cotización para los siguientes productos";

                Table table = new Table(4);
                Style style = new Style();

                //style.SetFontFamily("Helvetica");
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

                //table.AddCell("Producto 1");
                //table.AddCell("Bultos");
                //table.AddCell("4");

                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(imagePath));
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
                Paragraph p = new Paragraph();
                
                p.Add(image);
                
                var dateParagraph = new Paragraph(dateLine);
                dateParagraph.SetFontSize(11);
                var titlePragraph = new Paragraph(titleLine);
                titlePragraph.SetFontSize(11);
                var supplierParagraph = new Paragraph(supplierLine);
                supplierParagraph.SetFontSize(11);


                document.Add(p);
                document.Add(dateParagraph);
                document.Add(titlePragraph);
                document.Add(new Paragraph());
                document.Add(new Paragraph());

                document.Add(supplierParagraph);
                document.Add(table); 
                document.Close();

                string array = Convert.ToBase64String(memoryStream.ToArray());

                File.WriteAllBytes(filePath, memoryStream.ToArray());
            }

        }

    }

}