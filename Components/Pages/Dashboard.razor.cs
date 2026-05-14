

using Microsoft.AspNetCore.Components;
using MyPlanner.Models;

namespace MyPlanner.Components.Pages;

public partial class Dashboard : ComponentBase
{
    [Parameter]
    public bool ShowDeadlineFilter { get; set; }
    [Parameter]
    public List<Activity> Activities { get; set; }
    [Parameter] 
    public string Owner { get; set; }

    [Parameter]
    public EventCallback<Activity> OnEditActivity { get; set; }
    [Parameter]
    public EventCallback<Activity> OnDeleteActivity { get; set; }

    private DateRangeFilterModel filter = null;
    private List<Activity> FilteredActivities 
        => filter == null ? Activities 
        : Activities.Where(a => 
            (filter.StartDate == null || a.Deadline >= filter.StartDate) 
            && (filter.EndDate == null || a.Deadline <= filter.EndDate)
        ).ToList();

    void EditActivity(Activity activity) 
    {
        OnEditActivity.InvokeAsync(activity);
    }
    void DeleteActivity(Activity activity)
    {
        OnDeleteActivity.InvokeAsync(activity);
    }
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    private void HandleFilterChanged(DateRangeFilterModel newFilter)
    {
        filter = newFilter;
        StateHasChanged();
    }
}










