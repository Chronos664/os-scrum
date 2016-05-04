//Dialogs
function PBIDialog(type) {
    var addProductBacklogItemDialogData = {
        dialogID: "addPBI-dialog",
        dialogTitle: "Add Product Backlog Item",
        dialogItems: [
            new TextBox("pbiNameTextBox", true, "Name"),
            new TextBox("pbiDescriptionTextBox", true, "Description"),
            new TextBox("pbiEffortScoreTextBox", true, "Effort"),
            new StateList("pbiState", true, "Backlog Item State")
        ]
    };
    var addProductBacklgItemDialogSettings = {
        height: 340,
        width: 350,
        buttons: {
            "Add Product Backlog Item": function () {
                loadingOn();
                var newPBI = {
                    ProductBacklogItemID: 0,
                    ProjectId: $("#addPBI-dialog").data("id"),
                    Name: $("#pbiNameTextBox").val(),
                    Data: null,
                    Description: $("#pbiDescriptionTextBox").val(),
                    SprintID: null,
                    EffortScore: $("#pbiEffortScoreTextBox").val(),
                    Priority: 0,
                    State: $("#pbiState").val(),
                    DateFinished: null
                };
                postToAPI(newPBI, "ProductBacklog", $(this));
            }
        },
        "Cancel": function () {
            $(this).dialog("close");
        }
    };
    var editProductBacklogItemDialogData = {
        dialogID: "editPBI-dialog",
        dialogTitle: "Edit Product Backlog Item",
        dialogItems: [
            new TextBox("editpbiNameTextBox", true, "Name"),
            new TextBox("editpbiDescriptionTextBox", true, "Description"),
            new TextBox("editpbiEffortScoreTextBox", true, "Effort"),
            new StateList("editpbiState", true, "Backlog Item State")]
    };
    var editProductBacklgItemDialogSettings = {
        height: 340,
        width: 350,
        buttons: {
            "Update Product Backlog Item": function () {
                loadingOn();
                var newPBI = {
                    ProductBacklogItemID: $("#editPBI-dialog").data("pdata").pbiID,
                    ProjectId: $("#editPBI-dialog").data("pdata").projID,
                    Name: $("#editpbiNameTextBox").val(),
                    Data: null,
                    Description: $("#editpbiDescriptionTextBox").val(),
                    SprintID: $("#editPBI-dialog").data("pdata").SprintID,
                    EffortScore: $("#editpbiEffortScoreTextBox").val(),
                    Priority: $("#editPBI-dialog").data("pdata").priority,
                    State: $("#editpbiState").val()
                };
                putToAPI(newPBI, "ProductBacklog", newPBI.ProductBacklogItemID, $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    switch (type) {
        case "add":
            this.generatedDialog = new generateDialog(addProductBacklogItemDialogData, addProductBacklgItemDialogSettings);
            this.dialogOptions = addProductBacklgItemDialogSettings;
            break;
        case "edit":
            this.generatedDialog = new generateDialog(editProductBacklogItemDialogData, editProductBacklgItemDialogSettings);
            this.dialogOptions = editProductBacklgItemDialogSettings;
            break;
    }
    return this;
}

function ProjectDialog(type) {
    var AddData = {
        dialogID: "AddProject-dialog",
        dialogTitle: "Add Project",
        dialogItems: [
            new TextBox("projectNameTextBox", true, "Name of Project"),
            new TextBox("projectDescription-TextBox", true, "Project Description")
        ]
    };
    var EditData = {
        dialogID: "EditProject-dialog",
        dialogTitle: "Edit Project",
        dialogItems: [
            new TextBox("projectNameEditTextBox", true, "Name of Project"),
            new TextBox("projectDescriptionEdit-TextBox", true, "Project Description")
        ]
    };
    var AddSettings = {
        height: 250,
        width: 350,
        buttons: {
            "Add Project": function () {
                loadingOn();
                var newProject = {
                    ProjectId: 0,
                    ProjectName: $("#projectNameTextBox").val(),
                    ProjectDetails: $("#projectDescription-TextBox").val()
                }
                postToAPI(newProject, "Project", $(this));
            }
        },
        "Cancel": function () {
            $(this).dialog("close");
        }
    };
    var EditSettings = {
        height: 250,
        width: 350,
        buttons: {
            "Edit Project": function () {
                loadingOn();
                var projectToEdit = {
                    ProjectId: $("#EditProject-dialog").data("id"),
                    ProjectName: $("#projectNameEditTextBox").val(),
                    ProjectDetails: $("#projectDescriptionEdit-TextBox").val()
                }
                putToAPI(projectToEdit, "Project", projectToEdit.ProjectId, $(this));
            }
        },
        "Cancel": function () {
            $(this).dialog("close");
        }
    };
    var restoreProjectdata = {
        dialogID: "restoreProject-dialog",
        dialogTitle: "Restore Project",
        dialogItems: [
            new AlertMessage("Are you sure you want to restore this project?", "confirmAlert", false)
        ]
    };
    var restoreProjectSettigs = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function() {
                loadingOn();
                var url = "Project/Restore/" + $(this).data("pid");
                postToAPI(null, url, $(this));
            },
            "No": function() {
                $(this).dialog("close");
            }
        }
    };

    var archiveProjectdata = {
        dialogID: "archiveProject-dialog",
        dialogTitle: "Archive Project",
        dialogItems: [
            new AlertMessage("Are you sure you want to archive this project?", "confirmAlert", false)
        ]
    };
    var archiveProjectSettigs = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function () {
                loadingOn();
                var id = $(this).data("pid");
                deleteFromAPI("Project", id, $(this));
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    };


    switch (type) {
        case "add":
            this.generatedDialog = generateDialog(AddData, AddSettings);
            this.dialogOptions = AddData;
            break;
        case "edit":
            this.generatedDialog = generateDialog(EditData, EditSettings);
            this.dialogOptions = EditData;
            break;
        case "restore":
            this.generatedDialog = generateDialog(restoreProjectdata, restoreProjectSettigs);
            this.dialogOptions = restoreProjectdata;
            break;
        case "archive":
            this.generatedDialog = generateDialog(archiveProjectdata, archiveProjectSettigs);
            this.dialogOptions = archiveProjectdata;
            break;
    }
    return this;
}

function TaskDialog(type, projectID) {
    var AddTaskSetup = {
        dialogID: "AddTaskDialog",
        dialogTitle: "Add Task",
        dialogItems: [
            new TextBox("taskNameTextBox", true, "Name"),
            new TextBox("taskDescriptionTextBox", true, "Description"),
            new TextBox("taskTimeRemainingTextBox", true, "Time Remaining"),
            new StateList("taskState", true, "State"),
            new UserList("taskuserlist", true, "Assign User", projectID)
        ]
    };
    var EditTaskSetup = {
        dialogID: "EditTaskDialog",
        dialogTitle: "Edit Task",
        dialogItems: [
            new TextBox("edittaskNameTextBox", true, "Name"),
            new TextBox("edittaskDescriptionTextBox", true, "Description"),
            new TextBox("edittaskTimeRemainingTextBox", true, "Time Remaining"),
            new StateList("edittaskState", true, "State"),
            new UserList("edittaskuserlist", true, "Assign User", projectID)
        ]
    };
    var AddSettings = {
        height: 340,
        width: 350,
        buttons: {
            "Add Task": function () {
                loadingOn();
                var newTask = {
                    BacklogItemTaskID: 0,
                    ProductBacklogID: $("#AddTaskDialog").data("pbiid"),
                    Name: $("#taskNameTextBox").val(),
                    Description: $("#taskDescriptionTextBox").val(),
                    RemainingTime: $("#taskTimeRemainingTextBox").val(),
                    State: $("#taskState").val(),
                    CurrentUserID: $("#taskuserlist").val()
                };
                postToAPI(newTask, "BacklogTasks", $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    var EditSettings = {
        height: 340,
        width: 350,
        buttons: {
            "Edit Task": function () {
                loadingOn();
                var TaskToEdit = {
                    BacklogItemTaskID: $("#EditTaskDialog").data("tdata").taskid,
                    ProductBacklogID: $("#EditTaskDialog").data("tdata").pbiid,
                    Name: $("#edittaskNameTextBox").val(),
                    Description: $("#edittaskDescriptionTextBox").val(),
                    RemainingTime: $("#edittaskTimeRemainingTextBox").val(),
                    State: $("#edittaskState").val(),
                    CurrentUserID: $("#edittaskuserlist").val()

                };
                putToAPI(TaskToEdit, "BacklogTasks", TaskToEdit.BacklogItemTaskID, $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    switch (type) {
        case "add":
            this.generatedDialog = generateDialog(AddTaskSetup, AddSettings);
            this.dialogObjects = AddTaskSetup;
            break;
        case "edit":
            this.generatedDialog = generateDialog(EditTaskSetup, EditSettings);
            this.dialogObjects = EditTaskSetup;
            break;
    }
    return this;
}

function IterationDialog(type, projectID) {
    var AddDialog = {
        dialogID: "AddIteration-dialog",
        dialogTitle: "Add Iteration",
        dialogItems: [
            new TextBox("iterationNameTextBox", true, "Iteration Name"),
            new DatePicker("iterationStartDatePicker", true, "Select Start Date"),
            new DatePicker("iterationEndDatePicker", true, "Select End Date")
        ]
    };
    var EditDialog = {
        dialogID: "EditIteration-dialog",
        dialogTitle: "Edit Iteration",
        dialogItems: [
            new TextBox("edititerationNameTextBox", true, "Iteration Name"),
            new DatePicker("edititerationStartDatePicker", true, "Select Start Date"),
            new DatePicker("edititerationEndDatePicker", true, "Select End Date")
        ]
    };
    var AddSettings = {
        height: 300,
        width: 350,
        buttons: {
            "Add Iteration": function () {
                loadingOn();
                var newIteration = {
                    IterationID: 0,
                    ProjectID: $("#AddIteration-dialog").data("pid"),
                    SprintName: $("#iterationNameTextBox").val(),
                    SprintStartDate: $("#iterationStartDatePicker").val(),
                    SprintEndDate: $("#iterationEndDatePicker").val(),
                    Archived: 0
                };
                postToAPI(newIteration, "Iterations", $(this));
            },
            "Close": function () {
                $(this).dialog("close");
            }
        }
    };
    var EditSettings = {
        height: 300,
        width: 350,
        buttons: {
            "Update Iteration": function () {
                loadingOn();
                var editIteration = {
                    IterationID: $("#EditIteration-dialog").data("iid"),
                    ProjectID: $("#EditIteration-dialog").data("pid"),
                    SprintName: $("#edititerationNameTextBox").val(),
                    SprintStartDate: $("#edititerationStartDatePicker").val(),
                    SprintEndDate: $("#edititerationEndDatePicker").val(),
                    Archived: 0
                };
                putToAPI(editIteration, "Iterations", editIteration.IterationID, $(this));
            },
            "Close": function () {
                $(this).dialog("close");
            }
        }
    };
    var AddPBIDialog = {
        dialogID: "AddPBItoIteration-dialog",
        dialogTitle: "Add Project to Iteration",
        dialogItems: [
            new IterationList("iterationList", true, "Select Iteration", projectID)
        ]
    }
    var AddPBISettings = {
        height: 250,
        width: 350,
        buttons: {
            "Add To Iteration": function () {
                loadingOn();
                var addtoIteraton = {
                    PbiId: $("#AddPBItoIteration-dialog").data("pbiid"),
                    IterationId: $("#iterationList").val()
                }
                postToAPI(addtoIteraton, "Iterations/AddPBItoIteration", $(this));
            }
        }
    }

    switch (type) {
        case "add":
            this.generatedDialog = generateDialog(AddDialog, AddSettings);
            this.dialogObjects = AddDialog;
            break;
        case "edit":
            this.generatedDialog = generateDialog(EditDialog, EditSettings);
            this.dia
            break;
        case "addPBI":
            this.generatedDialog = generateDialog(AddPBIDialog, AddPBISettings);
            break;
    }
    return this;
}

function UserDialog(type, userID) {
    this.EditUserInfoDialogElements = {
        dialogID: "editUserInfo-dialog",
        dialogTitle: "Edit User Information",
        dialogItems: [
            new TextBox("editUserFirstNametextbox", true, "Edit First Name", "FirstName"),
            new TextBox("editUserLastNametextbox", true, "Edit Last Name", "LastName"),
            new TextBox("editUserEmailtextbox", true, "Edit Email Address", "Email")
        ]
    };
    this.EditUserInfoDialogSettings = {
        height: 300,
        width: 350,
        buttons: {
            "Update User Info": function () {
                loadingOn();
                var updateData = {
                    UserID: $("#editUserInfo-dialog").data("udata").UserID,
                    UserName: $("#editUserInfo-dialog").data("udata").UserName,
                    FirstName: $("#editUserFirstNametextbox").val(),
                    LastName: $("#editUserLastNametextbox").val(),
                    EmailAddress: $("#editUserEmailtextbox").val()
                }
                putToAPI(updateData, "Users", updateData.UserID, $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    this.AddUserToTeamElements = {
        dialogID: "addUserToTeam-dialog",
        dialogTitle: "Add User to Team",
        dialogItems: [
            new TeamList("addusertoteamlist", true, "Select Team", userID)
        ]
    };
    this.AddUserToTeamSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Add User to Selected Team": function () {
                loadingOn();
                var data = {
                    userID: $(this).data("uid"),
                    teamID: $("#addusertoteamlist").val()
                }
                postToAPI(data, "Users/AddUserToTeam", $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    this.RemoveUserFromTeamElements = {
        dialogID: "removeUserFromTeam-dialog",
        dialogTitle: "Remove User From Team",
        dialogItems: [
            new AlertMessage("Are you sure you wish to remove this user from this team?", "confirmMessage", false)
        ]
    };
    this.RemoveUserFromTeamSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function () {
                loadingOn();
                var data = {
                    userID: $(this).data("uid"),
                    teamID: $(this).data("tid")
                }
                postToAPI(data, "Users/RemoveUserFromTeam", $(this));
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    };

    this.BlockUserElements = {
        dialogID: "blockUser-dialog",
        dialogTitle: "Block User",
        dialogItems: [
            new AlertMessage("Are you sure you want to block this user?", "confirmMessage", false)
        ]
    };
    this.UnblockUserElements = {
        dialogID: "unblockUser-dialog",
        dialogTitle: "Unblock User",
        dialogItems: [
            new AlertMessage("Are you sure you want to unblock this user?", "confirmMessage", false)
        ]
    };
    this.BlockUserSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function() {
                loadingOn();
                deleteFromAPI("Users/BlockUser/", $(this).data("uid"), $(this));
            },
            "No": function() {
                $(this).dialog("close");
            }
        }
    }
    this.UnblockUserSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function () {
                loadingOn();
                var urlToPost = "Users/UnblockUser/" + $(this).data("uid");
                postToAPI(null, urlToPost, $(this));
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    }



    switch (type) {
        case "editUser":
            this.generatedDialog = generateDialog(this.EditUserInfoDialogElements, this.EditUserInfoDialogSettings);
            break;
        case "addUserToTeam":
            this.generatedDialog = generateDialog(this.AddUserToTeamElements, this.AddUserToTeamSettings);
            break;
        case "removeUserFromTeam":
            this.generatedDialog = generateDialog(this.RemoveUserFromTeamElements, this.RemoveUserFromTeamSettings);
            break;
        case "blockUser":
            this.generatedDialog = generateDialog(this.BlockUserElements, this.BlockUserSettings);
            break;
        case "unblockUser":
            this.generatedDialog = generateDialog(this.UnblockUserElements, this.UnblockUserSettings);
            break;
    }
    return this;
}

function assignRoleDialog(type, userID) {
    this.assignRoleToUserDialogElements = {
        dialogID: "assignUserToRole-dialog",
        dialogTitle: "Gramt Role To User",
        dialogItems: [
            new RoleList("assignRoleList", true, "Select Role to Assign To User", userID)
        ]
    };
    this.assignRoleToUserDialogSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Add Role To User": function () {
                loadingOn();
                var data = {
                    userID: $("#assignUserToRole-dialog").data("uid"),
                    roleID: $("#assignRoleList").val()
                };
                postToAPI(data, "Users/AddRoleToUser", $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    this.removeExplicitRoleFromUserElements = {
        dialogID: "removeUserFromRole-dialog",
        dialogTitle: "Remove Role From User",
        dialogItems: [
            new AlertMessage("Are you sure you want to remove this role from the user?", "confirmMessage", false)
        ]
    };
    this.removeExplicitRoleFromUserSettings = {
        height: 220,
        width: 500,
        buttons: {
            "Yes": function () {
                loadingOn();
                var data = {
                    userID: $("#removeUserFromRole-dialog").data("uid"),
                    roleID: $("#removeUserFromRole-dialog").data("rid")
                }
                postToAPI(data, "Users/RemoveRoleFromUser", $(this));
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    }

    switch (type) {
        case "assign":
            this.generatedDialog = generateDialog(this.assignRoleToUserDialogElements, this.assignRoleToUserDialogSettings);
            break;
        case "remove":
            this.generatedDialog = generateDialog(this.removeExplicitRoleFromUserElements, this.removeExplicitRoleFromUserSettings);
            break;
    };
    return this;
}

function TeamDialog(type) {
    this.AddElements = [
        new TextBox("addTeamNameTextBox", true, "Team Name", "TeamName")
    ];
    this.EditElements = [
        new TextBox("editTeamNameTextBox", true, "Team Name", "TeamName")
    ];
    this.AddSetup = generateSettings("addTeam-dialog", "Add New Team", this.AddElements);
    this.EditSetup = generateSettings("editTeam-dialog", "Edit Existing Team", this.EditElements);
    this.AddActions = {
        height: 200,
        width: 350,
        buttons: {
            "Add Team": function () {
                loadingOn();
                var data = {
                    TeamID: 0,
                    TeamName: $("#addTeamNameTextBox").val(),
                    Archived: false
                };
                postToAPI(data, "Teams", $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    this.EditActions = {
        height: 200,
        width: 350,
        buttons: {
            "Edit Team": function () {
                loadingOn();
                var data = {
                    TeamID: $("#editTeam-dialog").data("tid"),
                    TeamName: $("#editTeamNameTextBox").val(),
                    Archived: false
                };
                putToAPI(data, "Teams", data.TeamID, $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    var message = "Are you sure you wish to " + type + " this team?";
    this.Elements = [
        new AlertMessage(message, "confirm", false)
    ];
    this.ArchiveTeamSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function () {
                loadingOn();
                deleteFromAPI("Teams/ArchiveTeam", $(this).data("tid"), $(this));
            },
            "No": function() {
                $(this).dialog("close");
            }
        }
    };
    this.RestoreTeamSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Yes": function () {
                var url = "Teams/RestoreTeam/" + $(this).data("tid");
                loadingOn();
                postToAPI(null, url, $(this));
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    };
    this.ArchiveTeamSetup = generateSettings("archiveTeam-dialog", "Archive Team", this.Elements);
    this.RestoreTeamSetup = generateSettings("restoreTeam-dialog", "Restore Team", this.Elements);

    switch (type) {
        case "add":
            this.generatedDialog = generateDialog(this.AddSetup, this.AddActions);
            break;
        case "edit":
            this.generatedDialog = generateDialog(this.EditSetup, this.EditActions);
            break;
        case "archive":
            this.generatedDialog = generateDialog(this.ArchiveTeamSetup, this.ArchiveTeamSettings);
            break;
        case "restore":
            this.generatedDialog = generateDialog(this.RestoreTeamSetup, this.RestoreTeamSettings);
            break;
    }
    return this;
}

function TeamToProjectDialog(type, teamID) {
    this.AddElements = [
        new ProjectList("projectList", true, "Select Project to add Team to", teamID)
    ];
    this.RemoveElements = [
        new AlertMessage("Are you sure you want to remove this team from this project?", "alertMessage", false)
    ];
    this.AddData = generateSettings("addTeamToProject-dialog", "Add Team to Project", this.AddElements);
    this.RemoveData = generateSettings("removeTeamFromProject-dialog", "Remove Team from Project", this.RemoveElements);
    this.AddSettings = {
        height: 200,
        width: 350,
        buttons: {
            "Add Team To Project": function () {
                loadingOn();
                var data = {
                    teamID: $("#addTeamToProject-dialog").data("tid"),
                    ProjectID: $("#projectList").val()
                }
                postToAPI(data, "Teams/TeamToProject", $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    this.RemoveSettings = {
        height: 250,
        width: 350,
        buttons: {
            "Remove Team From Project": function () {
                loadingOn();
                var ttpmap = {
                    teamID: $("#removeTeamFromProject-dialog").data("tid"),
                    ProjectID: $("#removeTeamFromProject-dialog").data("pid")
                }
                deleteWithDataFromAPI("Teams/TeamToProject", ttpmap, $(this));
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    };
    switch (type) {
        case "add":
            this.generatedDialog = generateDialog(this.AddData, this.AddSettings);
            break;
        case "remove":
            this.generatedDialog = generateDialog(this.RemoveData, this.RemoveSettings);
            break;
    }
    return this;
}

//Dialog Setup Code
function TextBox(id, labelRequired, labelVal, elementName) {
    this.type = "textbox";
    this.id = id;
    this.requireslabel = labelRequired;
    this.labelVal = labelVal;
    this.Name = elementName;
}

function DatePicker(id, labelRequired, labelVal, elementName) {
    this.type = "datepicker";
    this.id = id;
    this.requireslabel = labelRequired;
    this.labelVal = labelVal;
    this.Name = elementName;
}

function ListBox(id, labelRequired, labelVal, options, elementName) {
    this.type = "listbox";
    this.id = id;
    this.requireslabel = labelRequired;
    this.labelVal = labelVal;
    this.options = options;
    this.Name = elementName;
}

function ListBoxOption(value, displayText) {
    this.val = value;
    this.display = displayText;
}

function AlertMessage(message, elementName, isDeleteMessage) {
    this.type = "alert";
    this.labelVal = message;
    this.Name = elementName;
    this.isDeleteMessage = isDeleteMessage;
}

function StateList(id, labelRequired, labelVal, elementName) {
    return new ListBox(id, labelRequired, labelVal,
    [
        new ListBoxOption(0, "Not Started"),
        new ListBoxOption(1, "In Progress"),
        new ListBoxOption(2, "Done")
    ], elementName);
}

function DeleteDialog(id, title, type, url) {
    var dialogData = {
        dialogID: id,
        dialogTitle: title,
        dialogItems: [
            new AlertMessage(type, "DeleteMessage", true)
        ]
    };
    var dialogSettings = {
        height: 220,
        width: 500,
        buttons: {
            "Yes": function () {
                loadingOn();
                deleteFromAPI(url, $(this).data("id"), $(this));
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    }
    return generateDialog(dialogData, dialogSettings);
}

function IterationListOptions(ProjectID) {
    var options = [];
    var address = _urlString + "Iterations/GetProjectIteration/" + ProjectID;
    options.push(new ListBoxOption(null, "Not Assigned"));
    $.ajax({
        dataType: "json",
        url: address,
        data: null,
        async: false
    }
    ).done(function (data) {
        $.each(data, function (index) {
            var current = data[index];
            options.push(new ListBoxOption(current.IterationID, current.SprintName));
        });
    });
    return options;
}

function IterationList(id, labelRequired, labelVal, ProjectID) {
    var options = IterationListOptions(ProjectID);
    return new ListBox(id, labelRequired, labelVal, options);
}

function availableRoles(userID) {
    var roles = [];
    $.ajax({
        url: _urlString + "Roles/GetRolesForUser/" + userID,
        type: "GET",
        async: false,
        dataType: "json",
        data: null
    }).done(function (data) {
        $.each(data, function (index) {
            var current = data[index];
            roles.push({ roleID: current.RoleID, roleName: current.RoleName });
        });
    });
    return roles;
}

function RoleList(id, labelRequired, labelVal, userID) {
    var options = [];
    var roles = availableRoles(userID);
    $.each(roles, function (index) {
        var current = roles[index];
        options.push(new ListBoxOption(current.roleID, current.roleName));
    });
    return new ListBox(id, labelRequired, labelVal, options, "RoleList");
}

function UserListOptions(ProjectID) {
    var options = [];
    options.push(new ListBoxOption(0, "Not Assigned"));
    var address = _urlString + "Project/GetUsersOnProject/" + ProjectID;
    $.ajax({
        dataType: "json",
        url: address,
        data: null,
        async: false
    }).done(function (data) {
        $.each(data, function (index) {
            var current = data[index];
            var fullname = current.FirstName + " " + current.LastName;
            options.push(new ListBoxOption(current.UserID, fullname));
        });
    });
    return options;
}

function UserList(id, labelRequired, labelVal, ProjectID) {
    var options = UserListOptions(ProjectID);
    return new ListBox(id, labelRequired, labelVal, options, "UserList");
}

function ProjectList(id, labelRequried, labelVal, teamID) {
    var options = [];
    $.ajax({
        url: _urlString + "Project/GetAvailableProjectsForTeam/" + teamID,
        type: "GET",
        data: null,
        dataType: "json",
        async: false
    }).done(function (data) {
        $.each(data, function (index) {
            var current = data[index];
            if (current != null) {
                options.push(new ListBoxOption(current.ProjectId, current.ProjectName));
            };
        });
    });
    return new ListBox(id, labelRequried, labelVal, options, "AvailableProjectList");
}

function TeamList(id, labelRequired, labelVal, userID) {
    var options = [];
    $.ajax({
        url: _urlString + "Users/AvailableTeams/" + userID,
        type: "GET",
        data: null,
        dataType: "json",
        async: false
    }).done(function (data) {
        $.each(data, function (index) {
            var current = data[index];
            if (current != null) {
                options.push(new ListBoxOption(current.TeamID, current.TeamName));
            };
        });
    });
    return new ListBox(id, labelRequired, labelVal, options, "userTeamList");
}

function generateSettings(dialogID, dialogTitle, dialogItems) {
    return {
        dialogID: dialogID,
        dialogTitle: dialogTitle,
        dialogItems: dialogItems
    };
}

//Dialog Initialization and Div Creation Code
function generateDialog(data, dialogSettings) {

    function generateDialogData(data) {
        var resultDiv = '<div id="' + data.dialogID + '" title="' + data.dialogTitle + '" style="display: none">';
        resultDiv += '<form><fieldset>';
        jQuery.each(data.dialogItems, function (index, item) {
            var current = item;
            if (current.requireslabel) {
                resultDiv += '<label for="' + current.id + '" style="font-size: 12px;">' + current.labelVal + '</label>';
            }
            switch (current.type) {
                case "textbox":
                    resultDiv += handleTextBox(current);
                    break;
                case "listbox":
                    resultDiv += handleListBox(current);
                    break;
                case "datepicker":
                    resultDiv += handleDatePicker(current);
                    break;
                case "alert":
                    resultDiv += handleAlertInfo(current);
                    break;
            }
        });
        resultDiv += '</form></fieldset>';
        resultDiv += '</div>';
        return resultDiv;
    }

    function handleTextBox(dataVal) {
        var textBoxElement = '<input type="text" id="' + dataVal.id + '" class="text ui-widget-content ui-corner-all" />';
        return textBoxElement;
    }

    function handleListBox(dataVal) {
        var listElement = '<select id="' + dataVal.id + '">';
        jQuery.each(dataVal.options, function (index, item) {
            var current = item;
            listElement += '<option value="' + current.val + '">' + current.display + '</option>';
        });
        listElement += '</select>';
        return listElement;
    }

    function handleAlertInfo(dataVal) {
        var alertIcon = '<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>';
        if (dataVal.isDeleteMessage) {
            var element = alertIcon + 'Are you sure you want to archive this ' + dataVal.labelVal + '?</br> This Action requires Administrator permissions to undo.</p>';
            return element;
        } else {
            var element = alertIcon + dataVal.labelVal + '</p>';
            return element;
        }
    }

    function handleDatePicker(dataVal) {
        return handleTextBox(dataVal);
    }

    var dialog = generateDialogData(data);
    dialog = $(dialog).dialog({
        height: dialogSettings.height,
        width: dialogSettings.width,
        modal: true,
        autoOpen: false
    });
    dialog = $(dialog).dialog('option', 'buttons', dialogSettings.buttons);
    return dialog;
}


//Default Data Display for Edit Dialogs, may need to change this to make it work a bit better
function generateDefaultData(targetID, val) {
    return {
        fieldID: "#" + targetID,
        dataToPlace: val
    };
}
function populateDefaultData(data) {
    $.each(data, function (index, item) {
        $(item.fieldID).val(item.dataToPlace);
    });
}

function BuildDefaultData(dialogItem, val) {
    return generateDefaultData(dialogItem.id, val);
}

function getDialogItem(dialog, itemName) {
    var result;
    $.each(dialog.dialogItems, function (index, item) {
        if (item.Name === itemName) {
            result = item;
        }
    });
    return result;
}