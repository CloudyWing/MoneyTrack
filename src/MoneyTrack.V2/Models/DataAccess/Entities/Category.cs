//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CloudyWing.MoneyTrack.Models.DataAccess.Entities {
    [Table("Categories")]
    public class Category {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public long DisplayOrder { get; set; }
    }
}
