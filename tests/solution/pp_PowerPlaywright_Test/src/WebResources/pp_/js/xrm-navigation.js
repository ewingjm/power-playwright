function openEntityRecord(entityName) {
    naviagteTo({
        pageType: "entityrecord",
        entityName: entityName
    });
}

function openEntityList(entityName, viewId) {
    naviagteTo({
        pageType: "entitylist",
        entityName: entityName,
        viewId: viewId,
    });
}

function openDashbaord(dashboardId) {
    naviagteTo({
        pageType: "dashboard",
        dashboardId: dashboardId,
    });
}

function openWebResource(webresourceName) {
    naviagteTo({
        pageType: "webresource",
        webresourceName: webresourceName,
    });
}

function openCustomPage(name, entityName, recordId) {
    naviagteTo({
        pageType: "custom",
        name: name,
        entityName: entityName,
        recordId: recordId
    });
}

function naviagteTo(pageInput) {
    Xrm.Navigation.navigateTo(pageInput, { target: 2, width: 800, height: 600, position: 1, title: "Custom Dialog" });
}