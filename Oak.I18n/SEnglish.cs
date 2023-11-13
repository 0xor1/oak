using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> English =
        new()
        {
            { HomeHeader, new("Welcome to Oak!") },
            {
                HomeBody,
                new(
                    "This is a project management app where you organise tasks in a tree structure like a file directory. Each task gives a summary of the vital stats of the tasks beneath it, estimated and incurred time and costs etc."
                )
            },
            { Home, new("Home") },
            {
                StringValidation,
                new("Invalid string {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
            },
            { CopyToClipboardSuccess, new("Copied to clipboard") },
            { NotStarted, new("Not Started") },
            { Uploading, new("Uploading") },
            { OrgTooMany, new("You are already a member of too many Orgs") },
            { ProjectInvalidHoursPerDay, new("Hours per day must be between 1 and 24") },
            { ProjectInvalidDaysPerWeek, new("Days per week must be between 1 and 7") },
            { ProjectFileLimitExceeded, new("Project file limit exceeded {{FileLimit}}") },
            {
                TaskTooManyDescN,
                new(
                    "Too many descendants to get all of them, only valid on tasks with 1000 or fewer descendants"
                )
            },
            { TaskCantMoveRootProjectNode, new("Can't move root project node") },
            { TaskRecursiveLoopDetected, new("Move operation would result in recursive loop") },
            {
                TaskMovePrevSibParentMismatch,
                new("Previous sibling and parent ids are mismatched")
            },
            {
                TaskDeleteProjectAttempt,
                new("Can't delete project node from tasks endpoint, use project delete endpoint")
            },
            {
                TaskTooManyDescNToDelete,
                new("Can't delete a task with {{Max}} or more descendant tasks")
            },
            { VItemInvalidTimeInc, new("Time entry must be 1 to 1440 minutes") },
            { VItemInvalidCostInc, new("Cost entry must be more than 0") },
            { Display, new("Display") },
            { User, new("User") },
            { Time, new("Time") },
            { Cost, new("Cost") },
            { File, new("File") },
            { SubCounts, new("Sub Counts") },
            { Minutes, new("Minutes") },
            { Hours, new("Hours") },
            { Days, new("Days") },
            { Weeks, new("Weeks") },
            { Years, new("Years") },
            { Unassigned, new("Unassigned") },
            { Task, new("Task") },
            { Parallel, new("Parallel") },
            { Sequential, new("Sequential") },
            { Comment, new("Comment") },
            { Description, new("Description") },
            { NoDescription, new("No Description") },
            { NothingToSeeHere, new("Nothing to see here.") },
            { Loading, new("Loading") },
            { Min, new("Min") },
            { Max, new("Max") },
            { True, new("True") },
            { False, new("False") },
            { Required, new("Required") },
            { Public, new("Public") },
            { New, new("New") },
            { Edit, new("Edit") },
            { Create, new("Create") },
            { Delete, new("Delete") },
            { Cancel, new("Cancel") },
            { Confirm, new("Confirm") },
            { Name, new("Name") },
            { Currency, new("Currency") },
            { CreatedOn, new("Created On") },
            { HoursPerDay, new("Hours per Day") },
            { DaysPerWeek, new("Days per Week") },
            { StartOn, new("Start On") },
            { EndOn, new("End On") },
            { Note, new("Note") },
            { TimeMin, new("Time Min.") },
            { TimeEst, new("Time Est.") },
            { TimeInc, new("Time Inc.") },
            { CostEst, new("Cost Est.") },
            { CostInc, new("Cost Inc.") },
            { FileN, new("File Count") },
            { FileSize, new("File Size") },
            { ChildN, new("Children") },
            { DescN, new("Descendants") },
            { FileLimit, new("File Limit") },
            { Upload, new("Upload") },
            { Invite, new("Invite") },
            { Active, new("Active") },
            { OrgMyOrgs, new("My Organisations") },
            { OrgNoOrgs, new("No Organisations") },
            { OrgNewOrg, new("New Organisation") },
            { OrgUpdateOrg, new("Update Organisation") },
            {
                OrgConfirmDeleteOrg,
                new(
                    "<p>Are you sure you want to delete the organisation <strong>{{Name}}</strong>?</p><p>This can not be undone.</p>"
                )
            },
            { OrgName, new("Organisation Name") },
            { OrgYourName, new("Your Name") },
            { OrgNameProjects, new("{{Name}} Projects") },
            { OrgProjects, new("Projects") },
            { OrgNoProjects, new("No Projects") },
            { OrgNewProject, new("New Project") },
            { OrgUpdateProject, new("Update Project") },
            {
                OrgConfirmDeleteProject,
                new(
                    "<p>Are you sure you want to delete the project <strong>{{Name}}</strong>?</p><p>This can not be undone.</p>"
                )
            },
            { OrgMembers, new("Members") },
            { OrgNoMembers, new("No Members") },
            { OrgNewMember, new("New Member") },
            { OrgUpdateMember, new("Update Member") },
            {
                OrgConfirmDeactivateMember,
                new(
                    "<p>Are you sure you want to deactivate the member <strong>{{Name}}</strong>?</p>"
                )
            },
            { OrgMemberInviteEmailSubject, new("{{OrgName}} - Project Management Invite") },
            {
                OrgMemberInviteEmailHtml,
                new(
                    "<p>Dear <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> has invited you to join the organisation: <strong>{{OrgName}}</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Please click this link to verify your email address and join <strong>{{OrgName}}</strong></a></p>"
                )
            },
            {
                OrgMemberInviteEmailText,
                new(
                    "Dear {{InviteeName}}\n\n{{InvitedByName}} has invited you to join the organisation: {{OrgName}}\n\nPlease click this link to verify your email address and join {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
                )
            },
            { OrgMemberRole, new("Role") },
            { OrgMemberRoleOwner, new("Owner") },
            { OrgMemberRoleAdmin, new("Admin") },
            { OrgMemberRoleWriteAllProjects, new("Write All Projects") },
            { OrgMemberRoleReadAllProjects, new("Read All Projects") },
            { OrgMemberRolePerProject, new("Per Project") },
            { TaskNewTask, new("New Task") },
            { TaskUpdateTask, new("Update Task") },
            {
                TaskConfirmDeleteTask,
                new(
                    "<p>Are you sure you want to delete the task <strong>{{Name}}</strong>?</p><p>This can not be undone.</p>"
                )
            },
            { TaskMovingTask, new("Moving Task: <strong>{{Name}}</strong>") },
            { TaskNewTime, new("New Time") },
            { TaskNewCost, new("New Cost") },
            { TaskUpdateTime, new("Update Time") },
            { TaskUpdateCost, new("Update Cost") },
            { TaskNoTimes, new("No Times") },
            { TaskNoCosts, new("No Costs") },
            {
                TaskConfirmDeleteVItem,
                new("<p>Are you sure you want to delete <strong>{{Value}}</strong>?</p>")
            },
            { TaskNoFiles, new("No Files") },
            {
                TaskConfirmDeleteFile,
                new("<p>Are you sure you want to delete the file <strong>{{Name}}</strong>?</p>")
            },
            { TaskUploadFile, new("Upload File") }
        };
}
