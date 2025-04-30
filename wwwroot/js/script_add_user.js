function addRole() {
    document.getElementById('role-selection').style.display = 'block';
}

function addDepartment() {
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
}

function selectDepartment(department) {
    const departmentsDiv = document.getElementById('departments');
    const newDepartment = document.createElement('div');
    newDepartment.classList.add('tag-container');
    newDepartment.innerHTML = `<span class="tag">${department}</span><button type="button" class="remove-btn" onclick="removeTag(this)">-</button>`;
    departmentsDiv.insertBefore(newDepartment, departmentsDiv.lastElementChild);
    updateHiddenInputs('departments');
    closeModal('department-selection');
}

function removeTag(button) {
    const tagContainer = button.parentElement;
    tagContainer.remove();
    updateHiddenInputs('roles');
    updateHiddenInputs('departments');
}

function updateHiddenInputs(type) {
    const container = document.getElementById(type);
    const tags = container.getElementsByClassName('tag');
    let tagsArray = [];
    for (let tag of tags) {
        tagsArray.push(tag.innerText);
    }
    document.getElementById(type + 'Input').value = tagsArray.join(',');
}
