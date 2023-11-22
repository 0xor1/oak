// Generated Code File, Do Not Edit.
// This file is generated with Common.I18nCodeGen.

using Common.Shared;

namespace Oak.I18n;

public static partial class S
{
    private static readonly Dictionary<string, TemplatableString> EN_Strings = new Dictionary<
        string,
        TemplatableString
    >()
    {
        { Active, new("Active") },
        { Add, new("Add") },
        { Cancel, new("Cancel") },
        { ChildN, new("Children") },
        { Clear, new("Clear") },
        { Comment, new("Comment") },
        { Confirm, new("Confirm") },
        { CopyToClipboardSuccess, new("Copied to clipboard") },
        { Cost, new("Cost") },
        { CostEst, new("Cost Est.") },
        { CostInc, new("Cost Inc.") },
        { Create, new("Create") },
        { CreatedOn, new("Created On") },
        { Currency, new("Currency") },
        { Days, new("Days") },
        { DaysPerWeek, new("Days per Week") },
        { Delete, new("Delete") },
        { DescN, new("Descendants") },
        { Description, new("Description") },
        { Display, new("Display") },
        { Edit, new("Edit") },
        { EndOn, new("End On") },
        { EntityNameMembers, new("{{Name}} Members") },
        { False, new("False") },
        { File, new("File") },
        { FileLimit, new("File Limit") },
        { FileN, new("File Count") },
        { FileSize, new("File Size") },
        { Home, new("Home") },
        {
            HomeBody,
            new(
                "This is a project management app where you organise tasks in a tree structure like a file directory. Each task gives a summary of the vital stats of the tasks beneath it, estimated and incurred time and costs etc."
            )
        },
        { HomeHeader, new("Welcome to Oak!") },
        { Hours, new("Hours") },
        { HoursPerDay, new("Hours per Day") },
        { Invite, new("Invite") },
        { Loading, new("Loading") },
        { Max, new("Max") },
        { Min, new("Min") },
        { Minutes, new("Minutes") },
        { Name, new("Name") },
        { New, new("New") },
        { NoDescription, new("No Description") },
        { NotStarted, new("Not Started") },
        { Note, new("Note") },
        { NothingToSeeHere, new("Nothing to see here.") },
        {
            OrgConfirmDeactivateMember,
            new("<p>Are you sure you want to deactivate the member <strong>{{Name}}</strong>?</p>")
        },
        {
            OrgConfirmDeleteOrg,
            new(
                "<p>Are you sure you want to delete the organisation <strong>{{Name}}</strong>?</p><p>This can not be undone.</p>"
            )
        },
        {
            OrgConfirmDeleteProject,
            new(
                "<p>Are you sure you want to delete the project <strong>{{Name}}</strong>?</p><p>This can not be undone.</p>"
            )
        },
        {
            OrgMemberInviteEmailHtml,
            new(
                "<p>Dear <strong>{{InviteeName}}</strong></p><p><strong>{{InvitedByName}}</strong> has invited you to join the organisation: <strong>{{OrgName}}</strong></p><p><a href=\"{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}\">Please click this link to verify your email address and join <strong>{{OrgName}}</strong></a></p>"
            )
        },
        { OrgMemberInviteEmailSubject, new("{{OrgName}} - Project Management Invite") },
        {
            OrgMemberInviteEmailText,
            new(
                "Dear {{InviteeName}}\n\n{{InvitedByName}} has invited you to join the organisation: {{OrgName}}\n\nPlease click this link to verify your email address and join {{OrgName}}:\n\n{{BaseHref}}/verify_email?email={{Email}}&code={{Code}}"
            )
        },
        { OrgMemberRole, new("Role") },
        { OrgMemberRoleAdmin, new("Admin") },
        { OrgMemberRoleOwner, new("Owner") },
        { OrgMemberRolePerProject, new("Per Project") },
        { OrgMemberRoleReadAllProjects, new("Read All Projects") },
        { OrgMemberRoleWriteAllProjects, new("Write All Projects") },
        { OrgMembers, new("Members") },
        { OrgMyOrgs, new("My Organisations") },
        { OrgName, new("Organisation Name") },
        { OrgNameProjects, new("{{Name}} Projects") },
        { OrgNewMember, new("New Member") },
        { OrgNewOrg, new("New Organisation") },
        { OrgNewProject, new("New Project") },
        { OrgNoMembers, new("No Members") },
        { OrgNoOrgs, new("No Organisations") },
        { OrgNoProjects, new("No Projects") },
        { OrgProjects, new("Projects") },
        { OrgTooMany, new("You are already a member of too many Orgs") },
        { OrgUpdateMember, new("Update Member") },
        { OrgUpdateOrg, new("Update Organisation") },
        { OrgUpdateProject, new("Update Project") },
        { OrgYourName, new("Your Name") },
        { Parallel, new("Parallel") },
        { ProjectFileLimitExceeded, new("Project file limit exceeded {{FileLimit}}") },
        { ProjectInvalidDaysPerWeek, new("Days per week must be between 1 and 7") },
        { ProjectInvalidHoursPerDay, new("Hours per day must be between 1 and 24") },
        { ProjectMemberRoleAdmin, new("Admin") },
        { ProjectMemberRoleWriter, new("Writer") },
        { ProjectMemberRoleReader, new("Reader") },
        { ProjectMembers, new("Project Members") },
        { Public, new("Public") },
        { Required, new("Required") },
        { Sequential, new("Sequential") },
        { StartOn, new("Start On") },
        {
            StringValidation,
            new("Invalid string {{Name}}, Min {{Min}}, Max {{Max}}, Regexes {{Regexes}}")
        },
        { SubCounts, new("Sub Counts") },
        { Task, new("Task") },
        { TaskCantMoveRootProjectNode, new("Can't move root project node") },
        {
            TaskConfirmDeleteFile,
            new("<p>Are you sure you want to delete the file <strong>{{Name}}</strong>?</p>")
        },
        {
            TaskConfirmDeleteTask,
            new(
                "<p>Are you sure you want to delete the task <strong>{{Name}}</strong>?</p><p>This can not be undone.</p>"
            )
        },
        {
            TaskConfirmDeleteVitem,
            new("<p>Are you sure you want to delete <strong>{{Value}}</strong>?</p>")
        },
        {
            TaskDeleteProjectAttempt,
            new("Can't delete project node from tasks endpoint, use project delete endpoint")
        },
        { TaskEditTask, new("Update Task") },
        { TaskMovePrevSibParentMismatch, new("Previous sibling and parent ids are mismatched") },
        { TaskMovingTask, new("Moving Task: <strong>{{Name}}</strong>") },
        { TaskNewCost, new("New Cost") },
        { TaskNewTask, new("New Task") },
        { TaskNewTime, new("New Time") },
        { TaskNoCosts, new("No Costs") },
        { TaskNoFiles, new("No Files") },
        { TaskNoTimes, new("No Times") },
        { TaskRecursiveLoopDetected, new("Move operation would result in recursive loop") },
        {
            TaskTooManyDescn,
            new(
                "Too many descendants to get all of them, only valid on tasks with 1000 or fewer descendants"
            )
        },
        {
            TaskTooManyDescnToDelete,
            new("Can't delete a task with {{Max}} or more descendant tasks")
        },
        { TaskUpdateCost, new("Update Cost") },
        { TaskUpdateTime, new("Update Time") },
        { TaskUploadFile, new("Upload File") },
        { Tasks, new("Tasks") },
        { Time, new("Time") },
        { TimeEst, new("Time Est.") },
        { TimeInc, new("Time Inc.") },
        { TimeMin, new("Time Min.") },
        { True, new("True") },
        { Unassigned, new("Unassigned") },
        { Upload, new("Upload") },
        { Uploading, new("Uploading") },
        { User, new("User") },
        { VitemInvalidCostInc, new("Cost entry must be more than 0") },
        { VitemInvalidTimeInc, new("Time entry must be 1 to 1440 minutes") },
        { Weeks, new("Weeks") },
        { Years, new("Years") },
    };
}
