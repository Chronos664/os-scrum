var _urlString = "";

function setState(eleType) {
    $(".state").each(function () {
        var stateValue = $(this).attr("stateVal");
        switch (stateValue) {
            case "0":
                $(this).append("<" + eleType + ">Not Started</" + eleType + ">");
                break;
            case "1":
                $(this).append("<" + eleType + ">In Progress</" + eleType + ">");
                break;
            case "2":
                $(this).append("<" + eleType + ">Done</" + eleType + ">");
                break;
        }
        return "true";
    });
};

function setupAccord(header, action, content) {
    $(action).click(function (e) {
        e.preventDefault();
        var parent = $(e.target).closest(header);
        parent.nextAll(content).eq(0).slideToggle("fast");
    });
}

function setStateExplicit(element) {
    $(element).each(function () {
        var stateValue = $(this).attr("stateVal");
        switch (stateValue) {
            case "0":
                $(this).append("Not Started");
                break;
            case "1":
                $(this).append("In Progress");
                break;
            case "2":
                $(this).append("Done");
                break;
        }
        return "true";
    });
};

function loadingOn() {
    $("body").addClass("loading");
};

function loadingOff() {
    setTimeout(function () {
        $("body").removeClass("loading");
    }, 800);
};

function postToAPI(dataToSend, url, dialog) {
    var urlString = _urlString + url;
    $.ajax({
        type: "POST",
        url: urlString,
        data: dataToSend,
    }).done(function (data, textStatus, xhr) {
        switch (xhr.status) {
            case 500:
                alert(data);
                if (dialog != null) {
                    dialog.dialog("close");
                }
                loadingOff();
            case 200:
                if (dialog != null) {
                    dialog.dialog("close");
                }
                location.reload(true);
                return true;
            default:
                var result = data + " " + xhr.status + " " + textStatus;
                loadingOff();
                alert(result);
                if (dialog != null) {
                    dialog.dialog("close");
                }
        }
    });
}
function tableSortHelper(event, ui) {
    ui.children().each(function () {
        $(this).Width($(this).width());
    });
    return ui;
}
//Not Currently In Use
function updateIterationTable(tableID, projID) {
    $.getJSON(_urlString + "Iterations/GetProjectIteration/" + projID, function (data) {
        formatIterationTable(tableID, data);
    });
}

function formatIterationTable(tableID, data) {
    $.each(data, function (index) {
        $(tableID).append('<tr><td class="iterationDroppable"><a href="/Project/IterationDetails/' + data[index].IterationID + '">' + data[index].SprintName + '</a></td></tr');
    });
}

function updateProjectTable(tableID) {
    $.getJSON(_urlString + "Project", function (data) {
        formatProjectTable(tableID, data);
    });
}


function formatProjectTable(tableID, data) {
    $.each(data, function (index) {
        $(tableID).append('<tr> <td> <a href="/Project/Details/' + data[index].ProjectId + '">' + data[index].ProjectName + '</a></td><td><p>' + data[index].ProjectDetails + '</p></td><td><a href="#" class="editProject" pid="' + data[index].ProjectId + '" pname="' + data[index].ProjectName + '" pdescription="' + data[index].ProjectDetails + '">Edit</a> | <a href="#" class="deleteProject" pid="' + data[index].ProjectId + '">Delete</a></td></tr>');
    });
}
//End not Currently Used

function getFromAPI(url) {
    var urlString = _urlString + url;
    $.ajax({
        url: urlString,
        type: "GET",
        async: false
    }).done(function(data) {
        return data;
    });
}

function getSingleFromAPI(url, itemID) {
    var urlString = _urlString + url + "/" + itemID;
    $.ajax({
        url: urlString,
        data: null,
        type: "json",
        async: false
    }).done(function (data) {
        return data;
    });
}

function putToAPI(dataToSend, url, itemID, dialog) {
    var urlString = _urlString + url + "/" + itemID;
    $.ajax(
    {
        type: "PUT",
        url: urlString,
        data: dataToSend,
    }).done(function (data, textStatus, xhr) {
        handleAjaxRequestFinish(data, textStatus, xhr, dialog);
    });
}

function deleteFromAPI(url, itemID, dialog) {
    var urlString = _urlString + url + "/" + itemID;
    $.ajax({
        type: "DELETE",
        url: urlString,
    }).done(function (data, textStatus, xhr) {
        handleAjaxRequestFinish(data, textStatus, xhr, dialog);
    });
}

function deleteWithDataFromAPI(url, data, dialog) {
    var urlString = _urlString + url;
    $.ajax({
        url: urlString,
        data: data,
        dataType: "json",
        type: "DELETE"
    }).done(function (data, textStatus, xhr) {
        handleAjaxRequestFinish(data, textStatus, xhr, dialog);
    });
}

function handleAjaxRequestFinish(data, textStatus, xhr, dialog) {
    if ((data == 0 || data == "Error" || data == null) && textStatus == "success") {
        loadingOff();
        alert("Internal Server Error, Please Contact your System Administrator");
        return false;
    }
    switch (xhr.status) {
        case 500:
            alert(data);
            dialog.dialog("close");
            loadingOff();
        case 200:
            dialog.dialog("close");
            location.reload(true);
            return true;
        default:
            var result = data + " " + xhr.status + " " + textStatus;
            loadingOff();
            alert(result);
            dialog.dialog("close");
    }
}

function loadPartialView(urlToLoad) {
    $.ajax({
        url: urlToLoad,
        type: "GET",
        contentType: "application/html",
        async: false
    }).done(function (content) {
        $("#partialContainer").html(content);
    });
}

function paginateTable(tableID, bodyID) {
    var currentPage = 0;
    var numPerPage = 5;
    var table = $(tableID);
    table.bind('re-paginate', function () {
        table.find(bodyID).hide().slice(currentPage * numPerPage, (currentPage + 1) * numPerPage).show();
    });
    table.trigger('re-paginate');
    var numRows = table.find(bodyID).length;
    var numPages = Math.ceil(numRows / numPerPage);
    var pager = $('<div class="pager"></div>');
    for (var page = 0; page < numPages; page++) {
        $('<span class="page-number"></span>').text(page + 1).bind('click', {
            newPage: page
        }, function (event) {
            currentPage = event.data['newPage'];
            table.trigger('re-paginate');
            $(this).addClass("active").siblings().removeClass("active");
        }).appendTo(pager).addClass("clickable");
    }
    pager.insertAfter(table).find('span.page-number:first').addClass('active');
}


function buildScrumBoard(projID) {
    function setupTableHeader(div) {
        $(div).append('<table class="scrumboard"><thead><tr><th>Items</th><th>Not Started</th><th>In Progress<th>Done</th></tr></thead><tbody id="scrumboardData"></tbody></table>');
    };
    function generateTableRow(data) {
        var row = '<tr>' +
            '<td>'
    };
    function formatPBI(data) {
        var bodyelement = $("#scrumboardData");
        var pbis = data.productBacklogItems;
        $.each(pbis, function(index) {
            var current = pbis[index];
        });
    }
    function populateData(data) {
        var scrumBoardDiv = $("#scrumBoard");
        setupTableHeader(scrumBoardDiv);
        formatPBI(data)
    };
    var iteration;
    var urlstring = _urlString + "Iterations/GetCurrentIterationForProject/" + projID;
    $.ajax({
        url: urlstring,
        type: "GET",
        async: false
    }).done(function (data, textStatus, xhr) {
        iteration = data;
        alert(iteration.productBacklogItems[0].PBITasks[0].Name);
        populateData(iteration);
    });

}
