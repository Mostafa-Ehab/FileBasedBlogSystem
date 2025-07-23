// Profile Page JavaScript
class ProfilePageManager {
    constructor() {
        this.init();

        this.user = null;
    }

    init() {
        this.setupEventListeners();
        this.loadUserProfile();
        this.setupPasswordValidation();
    }

    setupEventListeners() {
        // Profile picture upload
        const uploadInput = document.getElementById('profile-picture-upload');
        const preview = document.getElementById('profile-preview');

        if (uploadInput) {
            uploadInput.addEventListener('change', (e) => {
                this.handleProfilePictureUpload(e);
            });
        }

        // Form submissions
        const personalInfoForm = document.getElementById('personal-info-form');
        const securityForm = document.getElementById('security-form');

        if (personalInfoForm) {
            personalInfoForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handlePersonalInfoSubmit();
            });
        }

        if (securityForm) {
            securityForm.addEventListener('submit', (e) => {
                e.preventDefault();
                this.handleSecuritySubmit();
            });
        }

        // Reset buttons
        const resetPersonalInfo = document.getElementById('reset-personal-info');
        const cancelPasswordChange = document.getElementById('cancel-password-change');

        if (resetPersonalInfo) {
            resetPersonalInfo.addEventListener('click', () => {
                this.resetPersonalInfoForm();
            });
        }

        if (cancelPasswordChange) {
            cancelPasswordChange.addEventListener('click', () => {
                this.resetSecurityForm();
            });
        }

        // // Name field updates
        // const firstNameInput = document.getElementById('first-name');
        // const lastNameInput = document.getElementById('last-name');
        // const fullNameInput = document.getElementById('full-name');

        // if (firstNameInput && lastNameInput && fullNameInput) {
        //     firstNameInput.addEventListener('input', this.updateFullName);
        //     lastNameInput.addEventListener('input', this.updateFullName);
        // }

        // Bio character counter
        const bioTextarea = document.getElementById('bio');
        const bioCounter = document.getElementById('bio-count');

        if (bioTextarea && bioCounter) {
            bioTextarea.addEventListener('input', this.updateBioCounter);
        }

        // Password toggle buttons
        const passwordToggles = document.querySelectorAll('.password-toggle');
        passwordToggles.forEach(toggle => {
            toggle.addEventListener('click', this.togglePasswordVisibility);
        });

        // Custom form focus on TAB
        const formFields = document.querySelectorAll('input, select, textarea');
        formFields.forEach(field => {
            if (field.getAttribute('data-next')) {
                field.addEventListener('keydown', (e) => {
                    if (e.key === 'Tab' && !e.shiftKey) {
                        e.preventDefault();
                        const nextField = document.getElementById(field.getAttribute('data-next'));
                        if (nextField) {
                            nextField.focus();
                        }
                    }
                });
            }
        });
    }

    // Password validation and strength checking
    setupPasswordValidation() {
        const newPasswordInput = document.getElementById('new-password');
        const confirmPasswordInput = document.getElementById('confirm-password');
        const currentObject = this;

        if (newPasswordInput) {
            newPasswordInput.addEventListener('input', function () {
                currentObject.updatePasswordStrength(this.value);
                currentObject.updatePasswordRequirements(this.value);

                const confirmValue = confirmPasswordInput?.value || '';
                if (confirmValue) {
                    currentObject.updatePasswordMatchIndicator(this.value, confirmValue);
                }
            });
        }

        if (confirmPasswordInput) {
            confirmPasswordInput.addEventListener('input', function () {
                const newValue = newPasswordInput?.value || '';
                currentObject.updatePasswordMatchIndicator(newValue, this.value);
            });
        }
    }

    async loadUserProfile() {
        try {
            showLoading();
            const response = await getRequest('/api/users/me');
            this.user = response;
            this.populateForms();
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error loading data');
            } else {
                console.error('Error loading data:', error);
                showError('Error loading data');
            }
        } finally {
            hideLoading();
        }
    }

    // Profile picture handling
    async handleProfilePictureUpload(event) {
        const file = event.target.files[0];
        if (!file) return;

        // Validate file
        if (!this.validateProfilePicture(file)) {
            return;
        }

        // Preview the image
        const reader = new FileReader();
        reader.onload = function (e) {
            const preview = document.getElementById('profile-preview');
            if (preview) {
                preview.src = e.target.result;
            }
        };
        reader.readAsDataURL(file);

        // Upload the image
        this.uploadProfilePicture(file);
    }

    async uploadProfilePicture(file) {
        try {
            showLoading(true);

            const formData = new FormData();
            formData.append('profilePicture', file);

            const currentUser = getCurrentUser();
            const response = await putRequest(`/api/users/${currentUser.id}/profile-picture`,
                formData
            );

            const result = await response.json();

            if (result.success) {
                showMessage('Profile picture updated successfully', 'success');
            } else {
                showMessage(result.message || 'Failed to update profile picture', 'error');
            }
        } catch (error) {
            console.error('Error uploading profile picture:', error);
            showMessage('Error uploading profile picture', 'error');
        } finally {
            showLoading(false);
        }
    }

    async handlePersonalInfoSubmit() {
        const formData = new FormData(document.getElementById('personal-info-form'));
        const postData = Object.fromEntries(formData.entries());

        try {
            showLoading();
            const response = await putRequest('/api/users/me/profile-info', postData);
            this.user = response;
            setUser({ ...getUser(), ...response });
            showSuccess('Profile updated successfully');
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error updating profile');
            } else {
                console.error('Error updating profile:', error);
                showError('Error updating profile');
            }
        } finally {
            hideLoading();
        }
    }

    async handleSecuritySubmit() {
        const formData = new FormData(document.getElementById('security-form'));
        const postData = Object.fromEntries(formData.entries());

        try {
            showLoading();

            // Validate passwords match
            if (postData.newPassword !== postData.confirmPassword) {
                showError('New password and confirmation do not match');
                return;
            }

            // Validate password strength
            if (!this.isStrongPassword(postData.newPassword)) {
                showError('Password does not meet requirements', 'error');
                return;
            }

            await putRequest('/api/users/me/change-password', postData);
            showSuccess('Password changed successfully');
            this.resetSecurityForm();
        } catch (error) {
            if (error instanceof RequestError) {
                showError(error?.data?.message || 'Error changing password');
            } else {
                console.error('Error changing password:', error);
                showError('Error changing password');
            }
        } finally {
            hideLoading();
        }
    }

    populateForms() {
        const usernameField = document.getElementById('username');
        const emailField = document.getElementById('email');
        const fullNameField = document.getElementById('full-name');
        const bioField = document.getElementById('bio');

        usernameField.value = this.user.username || '';
        emailField.value = this.user.email || '';
        fullNameField.value = this.user.fullName || '';
        bioField.value = this.user.bio || '';

        // Profile picture
        const preview = document.getElementById('profile-preview');
        if (preview && this.user.profilePictureUrl) {
            preview.src = this.user.profilePictureUrl;
        }

        // Update bio counter
        this.updateBioCounter();
    }

    validateProfilePicture(file) {
        const maxSize = 5 * 1024 * 1024; // 5MB
        const allowedTypes = ['image/jpeg', 'image/png', 'image/gif'];

        if (file.size > maxSize) {
            showError('File size must be less than 5MB', 'error');
            return false;
        }

        if (!allowedTypes.includes(file.type)) {
            showError('Only JPEG, PNG, and GIF files are allowed', 'error');
            return false;
        }

        return true;
    }

    // Bio character counter
    updateBioCounter() {
        const bioTextarea = document.getElementById('bio');
        const bioCounter = document.getElementById('bio-count');

        if (bioTextarea && bioCounter) {
            const currentLength = bioTextarea.value.length;
            bioCounter.textContent = currentLength;

            // Update color based on length
            if (currentLength > 450) {
                bioCounter.style.color = '#dc3545';
            } else if (currentLength > 400) {
                bioCounter.style.color = '#ffc107';
            } else {
                bioCounter.style.color = '#007bff';
            }
        }
    }

    // Form reset functions
    resetPersonalInfoForm() {
        this.loadUserProfile();
    }

    resetSecurityForm() {
        const form = document.getElementById('security-form');
        if (form) {
            form.reset();
            this.updatePasswordStrength('');
            this.updatePasswordRequirements('');
            this.updatePasswordMatchIndicator('', '');
        }
    }

    // Password visibility toggle
    togglePasswordVisibility(event) {
        const button = event.currentTarget;
        const targetId = button.getAttribute('data-target');
        const input = document.getElementById(targetId);
        const icon = button.querySelector('i');

        if (input && icon) {
            if (input.type === 'password') {
                input.type = 'text';
                icon.className = 'fas fa-eye-slash';
            } else {
                input.type = 'password';
                icon.className = 'fas fa-eye';
            }
        }
    }

    // Password strength validation
    isStrongPassword(password) {
        const strength = this.calculatePasswordStrength(password);
        return strength.score >= 3; // Require at least "Good" strength
    }

    calculatePasswordStrength(password) {
        let score = 0;

        if (password.length >= 8) score++;
        if (/[a-z]/.test(password)) score++;
        if (/[A-Z]/.test(password)) score++;
        if (/\d/.test(password)) score++;
        if (/[^a-zA-Z\d]/.test(password)) score++;

        return { score, maxScore: 5 };
    }

    // Password Match Indicator
    updatePasswordMatchIndicator(newPassword, confirmPassword) {
        const indicator = document.getElementById('password-match-indicator');
        if (!indicator) return;

        if (!confirmPassword) {
            indicator.textContent = '';
            indicator.className = 'form-help';
            return;
        }

        if (newPassword === confirmPassword) {
            indicator.textContent = 'Passwords match';
            indicator.className = 'form-help success-text';
        } else {
            indicator.textContent = 'Passwords do not match';
            indicator.className = 'form-help error-text';
        }
    }

    updatePasswordStrength(password) {
        const strengthFill = document.getElementById('password-strength-fill');
        const strengthText = document.getElementById('password-strength-text');

        if (!strengthFill || !strengthText) return;

        const strength = this.calculatePasswordStrength(password);

        strengthFill.className = 'password-strength-fill';

        if (strength.score === 0) {
            strengthText.textContent = 'Password strength';
        } else if (strength.score < 2) {
            strengthFill.classList.add('weak');
            strengthText.textContent = 'Weak password';
        } else if (strength.score < 3) {
            strengthFill.classList.add('fair');
            strengthText.textContent = 'Fair password';
        } else if (strength.score < 4) {
            strengthFill.classList.add('good');
            strengthText.textContent = 'Good password';
        } else {
            strengthFill.classList.add('strong');
            strengthText.textContent = 'Strong password';
        }
    }

    // Password Requirements
    updatePasswordRequirements(password) {
        const requirements = [
            { id: 'req-length', test: password.length >= 8 },
            { id: 'req-lowercase', test: /[a-z]/.test(password) },
            { id: 'req-uppercase', test: /[A-Z]/.test(password) },
            { id: 'req-number', test: /\d/.test(password) },
            { id: 'req-special', test: /[^a-zA-Z\d]/.test(password) }
        ];

        requirements.forEach(req => {
            const element = document.getElementById(req.id);
            if (element) {
                const icon = element.querySelector('i');
                if (req.test) {
                    icon.className = 'fas fa-check text-success';
                } else {
                    icon.className = 'fas fa-times text-danger';
                }
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    window.profilePageManager = new ProfilePageManager();
});


// document.addEventListener('DOMContentLoaded', function () {
//     initializeProfilePage();
// });

// function initializeProfilePage() {
//     loadUserProfile();
//     setupEventListeners();
//     setupPasswordValidation();
//     setupFormValidation();
// }

// // Event Listeners
// function setupEventListeners() {
//     // Profile picture upload
//     const uploadInput = document.getElementById('profile-picture-upload');
//     const removeBtn = document.getElementById('remove-picture-btn');
//     const preview = document.getElementById('profile-preview');

//     if (uploadInput) {
//         uploadInput.addEventListener('change', handleProfilePictureUpload);
//     }

//     if (removeBtn) {
//         removeBtn.addEventListener('click', removeProfilePicture);
//     }

//     // Form submissions
//     const personalInfoForm = document.getElementById('personal-info-form');
//     const securityForm = document.getElementById('security-form');

//     if (personalInfoForm) {
//         personalInfoForm.addEventListener('submit', handlePersonalInfoSubmit);
//     }

//     if (securityForm) {
//         securityForm.addEventListener('submit', handleSecuritySubmit);
//     }

//     // Reset buttons
//     const resetPersonalInfo = document.getElementById('reset-personal-info');
//     const cancelPasswordChange = document.getElementById('cancel-password-change');

//     if (resetPersonalInfo) {
//         resetPersonalInfo.addEventListener('click', resetPersonalInfoForm);
//     }

//     if (cancelPasswordChange) {
//         cancelPasswordChange.addEventListener('click', resetSecurityForm);
//     }

//     // Name field updates
//     const firstNameInput = document.getElementById('first-name');
//     const lastNameInput = document.getElementById('last-name');
//     const fullNameInput = document.getElementById('full-name');

//     if (firstNameInput && lastNameInput && fullNameInput) {
//         firstNameInput.addEventListener('input', updateFullName);
//         lastNameInput.addEventListener('input', updateFullName);
//     }

//     // Bio character counter
//     const bioTextarea = document.getElementById('bio');
//     const bioCounter = document.getElementById('bio-count');

//     if (bioTextarea && bioCounter) {
//         bioTextarea.addEventListener('input', updateBioCounter);
//     }

//     // Password toggle buttons
//     const passwordToggles = document.querySelectorAll('.password-toggle');
//     passwordToggles.forEach(toggle => {
//         toggle.addEventListener('click', togglePasswordVisibility);
//     });

//     // Security options
//     const setup2faBtn = document.getElementById('setup-2fa');
//     const manageSessionsBtn = document.getElementById('manage-sessions');

//     if (setup2faBtn) {
//         setup2faBtn.addEventListener('click', setup2FA);
//     }

//     if (manageSessionsBtn) {
//         manageSessionsBtn.addEventListener('click', manageSessions);
//     }
// }

// // Load user profile data
// async function loadUserProfile() {
//     try {
//         showLoading(true);

//         // Get current user from auth
//         const currentUser = getCurrentUser();
//         if (!currentUser) {
//             window.location.href = '/admin/login.html';
//             return;
//         }

//         // Load user data
//         const response = await apiRequest(`/api/users/${currentUser.id}`, {
//             method: 'GET'
//         });

//         if (response.success) {
//             populateProfileForm(response.data);
//             updateLastLoginDisplay();
//         } else {
//             showMessage('Failed to load profile data', 'error');
//         }
//     } catch (error) {
//         console.error('Error loading profile:', error);
//         showMessage('Error loading profile data', 'error');
//     } finally {
//         showLoading(false);
//     }
// }

// // Populate form with user data
// function populateProfileForm(userData) {
//     // Personal information
//     setFieldValue('username', userData.username || '');
//     setFieldValue('email', userData.email || '');
//     setFieldValue('first-name', userData.firstName || '');
//     setFieldValue('last-name', userData.lastName || '');
//     setFieldValue('full-name', userData.fullName || '');
//     setFieldValue('bio', userData.bio || '');
//     setFieldValue('website', userData.website || '');

//     // Profile picture
//     if (userData.profilePicture) {
//         const preview = document.getElementById('profile-preview');
//         if (preview) {
//             preview.src = userData.profilePicture;
//         }
//     }

//     // Update bio counter
//     updateBioCounter();

//     // Update full name
//     updateFullName();
// }

// // Helper function to set field values
// function setFieldValue(fieldId, value) {
//     const field = document.getElementById(fieldId);
//     if (field) {
//         field.value = value;
//     }
// }

// // Profile picture handling
// function handleProfilePictureUpload(event) {
//     const file = event.target.files[0];
//     if (!file) return;

//     // Validate file
//     if (!validateProfilePicture(file)) {
//         return;
//     }

//     // Preview the image
//     const reader = new FileReader();
//     reader.onload = function (e) {
//         const preview = document.getElementById('profile-preview');
//         if (preview) {
//             preview.src = e.target.result;
//         }
//     };
//     reader.readAsDataURL(file);

//     // Upload the image
//     uploadProfilePicture(file);
// }



// async function uploadProfilePicture(file) {
//     try {
//         showLoading(true);

//         const formData = new FormData();
//         formData.append('profilePicture', file);

//         const currentUser = getCurrentUser();
//         const response = await fetch(`/api/users/${currentUser.id}/profile-picture`, {
//             method: 'POST',
//             body: formData,
//             headers: {
//                 'Authorization': `Bearer ${getAuthToken()}`
//             }
//         });

//         const result = await response.json();

//         if (result.success) {
//             showMessage('Profile picture updated successfully', 'success');
//         } else {
//             showMessage(result.message || 'Failed to update profile picture', 'error');
//         }
//     } catch (error) {
//         console.error('Error uploading profile picture:', error);
//         showMessage('Error uploading profile picture', 'error');
//     } finally {
//         showLoading(false);
//     }
// }

// function removeProfilePicture() {
//     if (confirm('Are you sure you want to remove your profile picture?')) {
//         const preview = document.getElementById('profile-preview');
//         if (preview) {
//             preview.src = 'https://picsum.photos/150/150?random=default';
//         }

//         // TODO: Make API call to remove profile picture
//         showMessage('Profile picture removed', 'success');
//     }
// }
