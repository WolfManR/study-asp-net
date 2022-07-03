using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using OfficeDashboard.Data;

namespace OfficeDashboard.Site.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly OfficeRepository _officeRepository;

        public EditModel(OfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        [ViewData]
        public SelectList OfficesSelectList { get; set; }

        [BindProperty]
        public Guid OriginalOfficeId { get; set; }

        [BindProperty]
        public EditEmployee Employee { get; set; }

        public async Task OnGetAsync(Guid id)
        {
            Employee = await _officeRepository.GetEmployeeForEdit(id);
            OriginalOfficeId = Employee.OfficeId;
            var offices = await _officeRepository.GetOffices();
            OfficesSelectList = new SelectList(offices, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _officeRepository.UpdateEmployeeData(Employee);

            return RedirectToPage("../Index", new { selectedOffice = OriginalOfficeId });
        }

        public IActionResult OnPostGoBack()
        {
            return RedirectToPage("../Index", new { selectedOffice = OriginalOfficeId });
        }
    }
}
