﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Storage")]
    public class Storage
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        [Required]
        public int StoreId { get; set; }
        [Required]
        public int MaterialID { get; set; }
        public virtual Store Stores { get; set; }
        public virtual Store Materials { get; set; }
    }
}