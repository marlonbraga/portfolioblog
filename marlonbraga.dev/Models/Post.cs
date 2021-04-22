using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marlonbraga.dev.Models {
	[Table("post")]
	public class Post {
		[Key, Display(Name = "Id")]
		public int IdPost { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime? PublicationDate { get; set; }
		[Required]
		public string Title { get; set; }
		public string TumbNail { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public string VideoLink { get; set; }

		public virtual List<PostTag> PostTags { get; set; }

		public List<Tag> Tags { get; set; }
		[NotMapped]
		public string TagsId { get; set; }

		[NotMapped]
		[DisplayName("Upload file")]
		public IFormFile ImageFile{ get; set; }
	}
}
