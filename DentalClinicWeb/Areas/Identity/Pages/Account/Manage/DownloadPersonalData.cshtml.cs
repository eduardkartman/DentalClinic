// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DentalClinicWeb.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace DentalClinicWeb.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            // Create a new PDF document
            Document document = new Document();

            // Create a new memory stream to write the PDF content
            MemoryStream memoryStream = new MemoryStream();

            // Create a PDF writer that writes to the memory stream
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Open the document
            document.Open();

            // Add content to the PDF document
            foreach (var entry in personalData)
            {
                document.Add(new Paragraph($"{entry.Key}: {entry.Value}"));
            }

            // Close the document
            document.Close();

            // Set the response headers to download the PDF file
            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.pdf");

            // Return the PDF file content as a FileContentResult
            return new FileContentResult(memoryStream.ToArray(), "application/pdf");
        }
    }
}
