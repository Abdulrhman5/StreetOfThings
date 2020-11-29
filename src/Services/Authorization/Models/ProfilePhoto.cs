    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ProfilePhoto : IEntity<int>
    {
        public int ProfilePhotoId { get; set; }

        public string FilePath { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime AddedAtUtc { get; set; }

        public int Id => ProfilePhotoId;
    }
}
