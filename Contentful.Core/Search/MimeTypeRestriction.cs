using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Search
{
    /// <summary>
    /// Represents the available mime types to restrict a search by.
    /// </summary>
    public enum MimeTypeRestriction
    {
        /// <summary>
        /// Attachments.
        /// </summary>
        Attachment,
        /// <summary>
        /// Plain text documents.
        /// </summary>
        Plaintext,
        /// <summary>
        /// Image files.
        /// </summary>
        Image,
        /// <summary>
        /// Audio files.
        /// </summary>
        Audio,
        /// <summary>
        /// Video files.
        /// </summary>
        Video,
        /// <summary>
        /// Rich text documents, such as MS Word or Pages.
        /// </summary>
        Richtext,
        /// <summary>
        /// Presentations such as PowerPoint or KeyNote.
        /// </summary>
        Presentation,
        /// <summary>
        /// Spreadsheets such as Excel or Numbers.
        /// </summary>
        Spreadsheet,
        /// <summary>
        /// Pdf documents.
        /// </summary>
        PdfDocument,
        /// <summary>
        /// Archive files such as .zip or .rar.
        /// </summary>
        Archive,
        /// <summary>
        /// Code files.
        /// </summary>
        Code,
        /// <summary>
        /// Markup files.
        /// </summary>
        Markup
    }
}
