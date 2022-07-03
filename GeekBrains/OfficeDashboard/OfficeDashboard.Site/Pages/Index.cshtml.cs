using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using OfficeDashboard.Data;

namespace OfficeDashboard.Site.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly OfficeRepository _officeRepository;

        public IndexModel(ILogger<IndexModel> logger, OfficeRepository officeRepository)
        {
            _logger = logger;
            _officeRepository = officeRepository;
        }

        [BindProperty]
        public Guid CurrentOfficeId { get; set; }

        [BindProperty]
        public CreateEmployee NewEmployee { get; set; }

        [ViewData]
        public SelectList OfficesSelectList { get; set; }

        [ViewData]
        public Guid SelectedOfficeId { get; set; }
        [ViewData]
        public Office SelectedOffice { get; set; }
        [ViewData]
        public IReadOnlyCollection<ListEmployee> NotAssignedToOfficeEmployees { get; set; }

        public async Task OnGetAsync(Guid selectedOffice)
        {
            var offices = await _officeRepository.GetOffices();
            OfficesSelectList = new SelectList(offices, "Id", "Name");
            if (selectedOffice != Guid.Empty)
            {
                CurrentOfficeId = selectedOffice;
                SelectedOffice = await _officeRepository.GetOffice(selectedOffice);
                SelectedOfficeId = SelectedOffice.Id;
            }

            if (await _officeRepository.IsContainsNotAssignedToOfficeEmployees())
            {
                NotAssignedToOfficeEmployees = await _officeRepository.GetEmployeesThatNotAssignedToAnyOffice();
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            return RedirectToPage(new { selectedOffice = CurrentOfficeId });
        }

        public async Task<IActionResult> OnPostCreate(Guid officeId)
        {
            if (!ModelState.IsValid) return Page();

            if (officeId != Guid.Empty) NewEmployee.OfficeId = officeId;
            await _officeRepository.RegisterEmployee(NewEmployee);

            return RedirectToPage(new { selectedOffice = officeId });
        }

        public async Task<IActionResult> OnPostDelete(Guid employeeId, Guid officeId)
        {
            await _officeRepository.RemoveEmployee(employeeId);

            return RedirectToPage(new { selectedOffice = officeId });
        }

        public IActionResult OnPostEdit(Guid employeeId)
        {
            return RedirectToPage("/Employees/Edit", new { id = employeeId });
        }

        public async Task<IActionResult> OnPostRemoveOffice()
        {
            await _officeRepository.RemoveOffice(CurrentOfficeId);

            return RedirectToPage(new { selectedOffice = CurrentOfficeId });
        }

        public IActionResult OnPostEditOffice()
        {
            return RedirectToPage("/Offices/Edit", new { id = CurrentOfficeId });
        }

        public IActionResult OnPostCreateOffice()
        {
            return RedirectToPage("/Offices/Create", new { openedOfficeId = CurrentOfficeId });
        }
    }
}