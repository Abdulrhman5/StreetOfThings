using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.TagCommands
{
    public class TagDeleter
    {
        private IRepository<int, Tag> _tagsRepo;

        private ILogger<TagDeleter> _logger;
        public TagDeleter(IRepository<int, Tag> tagsRepo, ILogger<TagDeleter> logger)
        {
            _tagsRepo = tagsRepo;
            _logger = logger;
        }

        public async Task<CommandResult> DeleteTag(DeleteTagDto deleteTagDto)
        {
            if(deleteTagDto == null ||  deleteTagDto.TagId == 0)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.DELETE.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var tag = _tagsRepo.Table.SingleOrDefault(t => t.TagId == deleteTagDto.TagId);
            if(tag is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.DELETE.NOTFOUND",
                    Message = "The tag you are trying to delete does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            try
            {
                tag.TagStatus = TagStatus.Deleted;
                await _tagsRepo.SaveChangesAsync();
                return new CommandResult();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "There were an error while deleting a tag {tagId}", deleteTagDto.TagId);
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.DELETE.ERROR",
                    Message = "There were an error while executing your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }
    }

    public class DeleteTagDto
    {
        public int TagId { get; set; }
    }
}
