using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using OfficeDashboard.Data;

namespace OfficeDashboard.Site.Pages.Offices
{
    public class CreateModel : PageModel
    {
        private readonly OfficeRepository _officeRepository;

        public CreateModel(OfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        [BindProperty]
        public CreateOffice Office { get; set; }

        [BindProperty]
        public Guid OpenedOfficeId { get; set; }

        public void OnGet(Guid openedOfficeId)
        {
            OpenedOfficeId = openedOfficeId;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var id = await _officeRepository.RegisterOffice(Office);
            var returnOfficeId = id == Guid.Empty ? OpenedOfficeId : id;
            return RedirectToPage("../Index", new { selectedOffice = returnOfficeId });
        }

        public IActionResult OnPostGoBack()
        {
            return RedirectToPage("../Index", new { selectedOffice = OpenedOfficeId });
        }
    }
}
