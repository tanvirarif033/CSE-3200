class NotificationManager {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.init();
    }

    async init() {
        try {
            await this.connection.start();
            console.log("SignalR Connected.");

            this.connection.on("ReceiveNotification", (notification) => {
                this.showNotification(notification);
                this.updateUnreadCount();
            });

            await this.updateUnreadCount();
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            setTimeout(() => this.init(), 5000);
        }
    }

    async updateUnreadCount() {
        try {
            const response = await fetch('/Notification/GetUnreadCount');
            const data = await response.json();

            this.updateBadgeCount(data.count);
        } catch (error) {
            console.error('Error updating unread count:', error);
        }
    }

    updateBadgeCount(count) {
        const badge = document.getElementById('notificationBadge');
        if (badge) {
            badge.textContent = count > 0 ? count : '';
            badge.style.display = count > 0 ? 'inline-block' : 'none';
        }
    }

    showNotification(notification) {
        const toast = document.createElement('div');
        toast.className = 'toast notification-toast';
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');

        toast.innerHTML = `
            <div class="toast-header bg-primary text-white">
                <strong class="me-auto">${this.escapeHtml(notification.title)}</strong>
                <small class="text-white">just now</small>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${this.escapeHtml(notification.message)}
                <div class="mt-2 pt-2 border-top">
                    <a href="/Disaster/Details/${notification.disasterId}" class="btn btn-sm btn-primary">View Details</a>
                </div>
            </div>
        `;

        const container = document.getElementById('notificationToasts') || document.body;
        container.appendChild(toast);

        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();

        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    async markAllAsRead() {
        try {
            const response = await fetch('/Notification/MarkAllAsRead', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.ok) {
                this.updateBadgeCount(0);
                location.reload();
            }
        } catch (error) {
            console.error('Error marking all as read:', error);
        }
    }
}

document.addEventListener('DOMContentLoaded', function () {
    window.notificationManager = new NotificationManager();
});