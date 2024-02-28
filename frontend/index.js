const searchInput = document.getElementById('floatingInputValue');
const searchResults = document.getElementById('employee-search-result-container');

searchInput.addEventListener('input', function() {
    const query = this.value.trim();
    if (query.length === 0) {
        searchResults.style.display = 'none';
        return;
    }

fetch(`http://localhost:8000/Employee/${query}`)
    .then(response => response.json())
    .then(data => {
    console.log(data);
    if (data.message !== "Employee not found") {
        displayResults(data);
    }})
    .catch(error => {
        console.error('Error fetching data:', error);
    });
});

function displayResults(results) {
    searchResults.innerHTML = '';
    var resultArr = [results];
    resultArr.forEach(result => {
        const resultItem = document.createElement('div');
        resultItem.classList.add('resultItem');
        resultItem.textContent = result.name;
        resultItem.addEventListener('click', function() {
        // Set the value of the search input to the clicked item's text
        searchInput.value = result.name;

        // Add functionality to the selected data
        //alert(You selected: ${result.title});
        // Hide the search results
        searchResults.style.display = 'none';
        console.log(result);
        displayEmployeeDetails(result);
        });
        searchResults.appendChild(resultItem);
    });
    searchResults.style.display = 'block';
}

// Close search results when clicking outside the search box
document.addEventListener('click', function(event) {
    if (!searchResults.contains(event.target) && event.target !== searchInput) {
        searchResults.style.display = 'none';
    }
});

function displayEmployeeDetails(data) {

    const card = document.querySelector('.card.border-primary');
    const employeeName = document.getElementById('card-employee-name');
    const employeeManagers = document.getElementById('employee-managers');
    const employeeDirectReports = document.getElementById('direct-reports');
    const employeeIndirectReports = document.getElementById('indirect-reports');
    const employeeDetails = document.getElementById('details');

    console.log(data);
    var managerNames = "N/A";
    var directReport = 0;
    var indirectReport = 0;
    var details = "-";

    if (data.isManagerAvailable && data.managers !=  null) {
        managerNames = data.managers.map(manager => {
            return Object.values(manager)[0];
            }).join(", ");
    }
    if (data.directReports !== null){
        directReport = data.directReports.length;
    }
    if (data.indirectReports !== null){
        indirectReport = data.indirectReports.length;
    }
    if (data.message !== "Request sucessfull"){
        details = data.message;
    }

    employeeName.textContent = data.name;
    employeeManagers.value = managerNames;
    employeeDirectReports.value = directReport;
    employeeIndirectReports.value = indirectReport;
    employeeDetails.value = details;
    card.style.display = "block";
    
}