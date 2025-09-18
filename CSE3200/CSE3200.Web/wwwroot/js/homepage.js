// wwwroot/js/homepage.js

// DISASTER ALERT TICKER FUNCTIONALITY
function initializeAlertTicker() {
    const tickerItems = $('.ticker-item');
    let currentIndex = 0;
    let isPaused = false;
    let tickerInterval;

    if (tickerItems.length > 0) {
        // Show first item
        $(tickerItems[currentIndex]).css('opacity', 1);

        // Start auto rotation if more than one alert
        if (tickerItems.length > 1) {
            startTicker();
        }

        // Next button
        $('#nextAlert').on('click', function () {
            if (!isPaused && tickerItems.length > 1) {
                clearInterval(tickerInterval);
            }
            showNextAlert();
            if (!isPaused && tickerItems.length > 1) {
                startTicker();
            }
        });

        // Previous button
        $('#prevAlert').on('click', function () {
            if (!isPaused && tickerItems.length > 1) {
                clearInterval(tickerInterval);
            }
            showPrevAlert();
            if (!isPaused && tickerItems.length > 1) {
                startTicker();
            }
        });

        // Pause/Play button
        $('#pausePlayAlert').on('click', function () {
            isPaused = !isPaused;
            if (isPaused) {
                clearInterval(tickerInterval);
                $(this).html('<i class="bi bi-play-fill"></i>');
            } else {
                startTicker();
                $(this).html('<i class="bi bi-pause-fill"></i>');
            }
        });
    }

    function startTicker() {
        tickerInterval = setInterval(function () {
            showNextAlert();
        }, 5000); // Rotate every 5 seconds
    }

    function showNextAlert() {
        $(tickerItems[currentIndex]).css('opacity', 0);
        currentIndex = (currentIndex + 1) % tickerItems.length;
        $(tickerItems[currentIndex]).css('opacity', 1);
    }

    function showPrevAlert() {
        $(tickerItems[currentIndex]).css('opacity', 0);
        currentIndex = (currentIndex - 1 + tickerItems.length) % tickerItems.length;
        $(tickerItems[currentIndex]).css('opacity', 1);
    }
}

// Search functionality
function initializeSearch() {
    $('#disasterSearch').on('keyup', function () {
        var searchText = $(this).val().toLowerCase();
        var severityFilter = $('#severityFilter').val();

        $('#disastersContainer .col-lg-4').each(function () {
            var cardText = $(this).text().toLowerCase();
            var cardSeverity = $(this).data('severity');
            var matchesSearch = searchText === '' || cardText.includes(searchText);
            var matchesSeverity = severityFilter === '' || cardSeverity === severityFilter;

            $(this).toggle(matchesSearch && matchesSeverity);
        });
    });

    // Severity filter
    $('#severityFilter').on('change', function () {
        $('#disasterSearch').trigger('keyup');
    });
}

// Smooth scroll for anchor links
function initializeSmoothScroll() {
    $('a[href^="#"]').on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $($(this).attr('href')).offset().top - 100
        }, 500);
    });
}

// Get anti-forgery token
function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// DONATION FUNCTIONALITY
function initializeDonation() {
    $('.donate-btn').on('click', function () {
        const disasterId = $(this).data('disaster-id');
        const disasterTitle = $(this).data('disaster-title');

        $('#donationDisasterId').val(disasterId);
        $('#donationDisasterTitle').val(disasterTitle);
        $('#donationDisasterName').text(disasterTitle);

        // Reset form
        $('#donationForm')[0].reset();
        $('#donationForm').removeClass('was-validated');
        $('#donationResult').addClass('d-none').removeClass('alert-success alert-danger');

        $('#donationModal').modal('show');
    });

    // Handle donation form submission
    $('#donationForm').on('submit', function (e) {
        e.preventDefault();

        const form = $(this)[0];
        if (!form.checkValidity()) {
            e.stopPropagation();
            $(this).addClass('was-validated');
            return;
        }

        const formData = $(this).serialize();
        const submitBtn = $(this).find('button[type="submit"]');

        // Show loading state
        submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');

        $.ajax({
            url: '/Home/Donate',
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': getAntiForgeryToken()
            },
            success: function (response) {
                if (response.success) {
                    $('#donationResult').removeClass('d-none alert-danger')
                        .addClass('alert-success')
                        .html(`
                            <i class="bi bi-check-circle-fill"></i>
                            <strong>Success!</strong> ${response.message}
                            <br><small>Transaction ID: ${response.transactionId}</small>
                            <br><small>Thank you for your generosity!</small>
                        `);

                    // Reset form and close modal after 3 seconds
                    setTimeout(() => {
                        $('#donationModal').modal('hide');
                        $('#donationForm')[0].reset();
                        $('#donationForm').removeClass('was-validated');
                        submitBtn.prop('disabled', false).html('<i class="bi bi-credit-card me-1"></i> Proceed to Payment');
                    }, 3000);
                } else {
                    $('#donationResult').removeClass('d-none alert-success')
                        .addClass('alert-danger')
                        .html(`
                            <i class="bi bi-exclamation-triangle-fill"></i>
                            <strong>Error:</strong> ${response.message}
                            ${response.errors ? '<br><small>' + response.errors.join('<br>') + '</small>' : ''}
                        `);
                    submitBtn.prop('disabled', false).html('<i class="bi bi-credit-card me-1"></i> Proceed to Payment');
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', status, error, xhr.responseText);
                $('#donationResult').removeClass('d-none alert-success')
                    .addClass('alert-danger')
                    .html(`
                        <i class="bi bi-exclamation-triangle-fill"></i>
                        <strong>Error:</strong> Something went wrong. Please try again.
                        <br><small>${xhr.status} ${xhr.statusText}</small>
                    `);
                submitBtn.prop('disabled', false).html('<i class="bi bi-credit-card me-1"></i> Proceed to Payment');
            }
        });
    });

    // Clear validation when modal is closed
    $('#donationModal').on('hidden.bs.modal', function () {
        $('#donationForm').removeClass('was-validated');
        $('#donationResult').addClass('d-none').removeClass('alert-success alert-danger');
    });
}

// DETAILS BUTTON FUNCTIONALITY
function initializeDetailsButtons() {
    $('.details-btn').on('click', function () {
        const disasterId = $(this).data('disaster-id');
        const disasterTitle = $(this).data('disaster-title');

        $('#detailsModal .modal-title').text('Details: ' + disasterTitle);

        // Show loading state
        $('#disasterDetails').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2">Loading disaster details...</p>
            </div>
        `);

        $('#detailsModal').modal('show');

        // Load disaster details
        $.ajax({
            url: '/Home/GetDisasterDetails',
            type: 'GET',
            data: { disasterId: disasterId },
            success: function (response) {
                if (response.success) {
                    const disaster = response.disaster;
                    const volunteers = response.volunteers;

                    let volunteersHtml = '';
                    if (volunteers.length > 0) {
                        volunteersHtml = `
                            <h5 class="mt-4"><i class="bi bi-people-fill me-2"></i>Volunteer Assignments (${volunteers.length})</h5>
                            <div class="table-responsive">
                                <table class="table table-sm modal-volunteer-table">
                                    <thead>
                                        <tr>
                                            <th>Volunteer Name</th>
                                            <th>Email</th>
                                            <th>Task Description</th>
                                            <th>Assigned Date</th>
                                            <th>Assigned By</th>
                                            <th>Status</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                        `;

                        volunteers.forEach(volunteer => {
                            volunteersHtml += `
                                <tr>
                                    <td>
                                        <div>${volunteer.volunteerName || 'Unknown Volunteer'}</div>

                                    </td>
                                    <td>${volunteer.volunteerEmail || 'N/A'}</td>
                                    <td>${volunteer.taskDescription}</td>
                                    <td>${volunteer.assignedDate}</td>
                                    <td>${volunteer.assignedBy}</td>
                                    <td><span class="badge bg-${volunteer.status === 'Assigned' ? 'success' :
                                    volunteer.status === 'Completed' ? 'primary' : 'warning'}">${volunteer.status}</span></td>
                                </tr>
                            `;
                        });

                        volunteersHtml += `
                                    </tbody>
                                </table>
                            </div>
                        `;
                    } else {
                        volunteersHtml = `
                            <div class="alert alert-info mt-4">
                                <i class="bi bi-info-circle"></i> No volunteers have been assigned to this disaster yet.
                            </div>
                        `;
                    }

                    $('#disasterDetails').html(`
                        <div>
                            <h4>${disaster.title}</h4>
                            <p><strong>Location:</strong> ${disaster.location}</p>
                            <p><strong>Occurred Date:</strong> ${disaster.occurredDate}</p>
                            <p><strong>Severity:</strong> <span class="badge bg-${disaster.severity === 'Critical' ? 'danger' :
                            disaster.severity === 'High' ? 'warning' :
                                disaster.severity === 'Medium' ? 'info' : 'success'}">${disaster.severity}</span></p>
                            <p><strong>Affected People:</strong> ${disaster.affectedPeople.toLocaleString()}</p>
                            <p><strong>Volunteers Assigned:</strong> <span class="badge volunteer-badge">${disaster.volunteerCount}</span></p>

                            <h5 class="mt-4">Description</h5>
                            <p>${disaster.description}</p>

                            <h5 class="mt-4">Required Assistance</h5>
                            <p>${disaster.requiredAssistance}</p>

                            ${volunteersHtml}
                        </div>
                    `);
                } else {
                    $('#disasterDetails').html(`
                        <div class="alert alert-danger">
                            <i class="bi bi-exclamation-triangle"></i> Error loading disaster details: ${response.message}
                        </div>
                    `);
                }
            },
            error: function (xhr, status, error) {
                $('#disasterDetails').html(`
                    <div class="alert alert-danger">
                        <i class="bi bi-exclamation-triangle"></i> Error loading disaster details. Please try again.
                    </div>
                `);
            }
        });
    });
}

// MAP FUNCTIONALITY
let map;
let marker;

function initializeMapButtons() {
    $('.map-btn').on('click', function () {
        const disasterId = $(this).data('disaster-id');
        const disasterTitle = $(this).data('disaster-title');
        const disasterLocation = $(this).data('disaster-location');

        $('#mapModal .modal-title').text('Location: ' + disasterTitle);
        $('#mapLocationInfo').html(`<p class="mb-0"><strong>Location:</strong> ${disasterLocation}</p>`);

        // Show loading state
        $('#mapContainer').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2">Loading map...</p>
            </div>
        `);

        $('#mapModal').modal('show');

        // Load map data
        $.ajax({
            url: '/Home/GetDisasterMap',
            type: 'GET',
            data: { id: disasterId },
            success: function (response) {
                if (response.success) {
                    // Update open in maps link
                    const encodedLocation = encodeURIComponent(response.location);
                    $('#openInMaps').attr('href', `https://www.google.com/maps/search/?api=1&query=${encodedLocation}`);

                    // Initialize map
                    initMap(response.mapUrl, response.coordinates, response.location);
                } else {
                    $('#mapContainer').html(`
                        <div class="alert alert-danger text-center py-4">
                            <i class="bi bi-exclamation-triangle"></i> Error loading map: ${response.message}
                        </div>
                    `);
                }
            },
            error: function (xhr, status, error) {
                $('#mapContainer').html(`
                    <div class="alert alert-danger text-center py-4">
                        <i class="bi bi-exclamation-triangle"></i> Error loading map. Please try again.
                    </div>
                `);
            }
        });
    });

    function initMap(mapUrl, coordinates, location) {
        // Clear previous map
        $('#mapContainer').html('<div id="map" style="height: 400px; width: 100%;"></div>');

        if (coordinates) {
            // Use coordinates if available for more precise mapping
            const [lat, lng] = coordinates.split(',').map(coord => parseFloat(coord));

            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: lat, lng: lng },
                zoom: 12,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });

            marker = new google.maps.Marker({
                position: { lat: lat, lng: lng },
                map: map,
                title: location
            });
        } else {
            // Fallback to embedded iframe if coordinates not available
            $('#mapContainer').html(`
                <iframe
                    width="100%"
                    height="400"
                    frameborder="0"
                    style="border:0"
                    src="${mapUrl}"
                    allowfullscreen>
                </iframe>
            `);
        }
    }

    // Clear map when modal is closed to avoid conflicts
    $('#mapModal').on('hidden.bs.modal', function () {
        if (map) {
            google.maps.event.clearInstanceListeners(map);
            map = null;
        }
        if (marker) {
            marker.setMap(null);
            marker = null;
        }
        $('#mapContainer').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2">Loading map...</p>
            </div>
        `);
    });
}

// Initialize all functionality when document is ready
$(document).ready(function () {
    initializeAlertTicker();
    initializeSearch();
    initializeSmoothScroll();
    initializeDonation();
    initializeDetailsButtons();
    initializeMapButtons();
});