using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marlonbraga.dev.Models {
	[Table("tag")]
	public class Tag {
		[Key, Display(Name = "Id")]
		public int IdTag { get; set; }
		[Required]
		public string Name { get; set; }
		public virtual List<PostTag> PostTags { get; set; }
	}
}
