function updateHiddenInputs(type) {
    const container = document.getElementById(type);
    const tags = container.getElementsByClassName('tag');
    let tagsArray = [];
    for (let tag of tags) {
        tagsArray.push(tag.innerText);
    }
    const inputValue = tagsArray.join(',');
    document.getElementById(type + 'Input').value = inputValue;
    console.log(`Updated ${type}: ${inputValue}`);
}

function removeTag(button) {
    const tagContainer = button.parentElement;
    const type = tagContainer.parentElement.id;
    tagContainer.remove();
    updateHiddenInputs(type);
    console.log(`Removed tag from ${type}`);
}

function addRole() {
    console.log('addRole function called');
    document.getElementById('role-selection').style.display = 'block';
}

function addDepartment() {
    console.log('addDepartment function called');
    document.getElementById('department-selection').style.display = 'block';
}

function closeModal(modalId) {
    document.getElementById(modalId).style.display = 'none';
}

function selectRole(role) {
    const rolesDiv = document.getElementById('roles');
    const newRole = document.createElement('div');
    newRole.classList.add('tag-container');
    newRole.innerHTML = `<span class="tag">${role}</span><button type="button" class="remove-btn" onclick="removeTag(this)">-</button>`;
    rolesDiv.insertBefore(newRole, rolesDiv.lastElementChild);
    updateHiddenInputs('roles');
    closeModal('role-selection');
    console.log('Selected Role: ${role}'); //debug
}

function selectDepartment(department) {
    const departmentsDiv = document.getElementById('departments');
    const newDepartment = document.createElement('div');
    newDepartment.classList.add('tag-container');
    newDepartment.innerHTML = `<span class="tag">${department}</span><button type="button" class="remove-btn" onclick="removeTag(this)">-</button>`;
    departmentsDiv.insertBefore(newDepartment, departmentsDiv.lastElementChild);
    updateHiddenInputs('departments');
    closeModal('department-selection');
    console.log('Selected Department: ${department}'); //debug
}

function initializeHiddenInputs() {
    console.log('initalizing hidden inputs'); //debug
    updateHiddenInputs('roles');
    updateHiddenInputs('departments');
}

document.addEventListener('DOMContentLoaded', function() {
    initializeHiddenInputs();
    
    // Add event listeners for the add buttons
    document.querySelector('#roles .add-btn').addEventListener('click', addRole);
    document.querySelector('#departments .add-btn').addEventListener('click', addDepartment);
    
    // Add event listeners for the close buttons in the modals
    document.querySelectorAll('.modal .close').forEach(closeBtn => {
        closeBtn.addEventListener('click', function() {
            closeModal(this.closest('.modal').id);
        });
    });
});