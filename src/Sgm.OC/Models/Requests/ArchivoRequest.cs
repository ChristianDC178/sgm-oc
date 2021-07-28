﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sgm.OC.Models.Requests
{
    public class ArchivoRequest
    {

        public int Id { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "File Name")]
        public string UntrustedName { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public DateTime UploadDT { get; set; }
        public decimal Cotizacion { get; set; }

    }
}
