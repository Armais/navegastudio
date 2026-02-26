// ── PeerEscrow Frontend (NavegaStudio Area) ─────────────
// MetaMask wallet connection + Demo mode fallback
// All contract interactions go through backend API in demo mode

'use strict';

const App = {
    walletAddress: null,
    isDemoMode: true,
    contractInfo: null,

    // ── Init ─────────────────────────────────────────────
    async init() {
        await this.loadContractInfo();
        this.setupDemoBadge();

        // Page-specific init
        if (document.getElementById('escrow-list')) {
            this.initDashboard();
        }
        if (document.getElementById('actions-card')) {
            this.initDetail();
        }
    },

    async loadContractInfo() {
        try {
            const res = await fetch('/api/escrow/contract-info');
            this.contractInfo = await res.json();
            this.isDemoMode = this.contractInfo.isDemoMode;
        } catch {
            this.isDemoMode = true;
        }
    },

    // ── Security ──────────────────────────────────────────
    escapeHtml(str) {
        if (!str) return '';
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    },

    setupDemoBadge() {
        const badge = document.getElementById('demo-badge');
        if (badge && this.isDemoMode) {
            badge.classList.remove('d-none');
        }
    },

    // ── Dashboard ────────────────────────────────────────
    initDashboard() {
        this.setupCreateForm();
        this.loadEscrows();
    },

    setupCreateForm() {
        const form = document.getElementById('form-create-escrow');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const btn = document.getElementById('btn-create');
            btn.disabled = true;
            btn.textContent = 'Creating...';

            const buyerAddr = document.getElementById('input-buyer').value.trim();
            if (!buyerAddr) {
                this.showToast('Please enter or connect a buyer address', 'warning');
                btn.disabled = false;
                btn.textContent = 'Create Escrow';
                return;
            }

            try {
                const res = await fetch('/api/escrow/create', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        sellerAddress: document.getElementById('input-seller').value.trim(),
                        arbiterAddress: document.getElementById('input-arbiter').value.trim(),
                        amount: parseFloat(document.getElementById('input-amount').value),
                        arbiterFeeBps: parseInt(document.getElementById('input-fee').value) || 100,
                        description: document.getElementById('input-description').value.trim(),
                        buyerAddress: buyerAddr
                    })
                });

                if (!res.ok) {
                    const err = await res.json();
                    throw new Error(err.error || 'Failed to create escrow');
                }

                const escrow = await res.json();
                this.showToast(`Escrow #${parseInt(escrow.id)} created successfully!`, 'success');
                form.reset();

                // Restore buyer address after reset
                document.getElementById('input-buyer').value = buyerAddr;

                // Collapse form and reload
                const collapse = bootstrap.Collapse.getInstance(document.getElementById('createForm'));
                if (collapse) collapse.hide();
                this.loadEscrows();
            } catch (err) {
                this.showToast(err.message, 'danger');
            } finally {
                btn.disabled = false;
                btn.textContent = 'Create Escrow';
            }
        });
    },

    async loadEscrows() {
        const container = document.getElementById('escrow-list');
        if (!container) return;

        try {
            let url = '/api/escrow/all';
            if (this.walletAddress) {
                url = `/api/escrow/user/${this.walletAddress}`;
            }

            const res = await fetch(url);
            const escrows = await res.json();

            if (escrows.length === 0) {
                container.innerHTML = `
                    <div class="text-center text-muted py-5">
                        <p class="fs-5 mb-2">No escrows yet</p>
                        <p>Create your first escrow or connect a wallet to see your transactions.</p>
                    </div>`;
                return;
            }

            container.innerHTML = escrows.map(e => this.renderEscrowCard(e)).join('');
        } catch (err) {
            container.innerHTML = `<div class="text-center text-danger py-5">Error loading escrows: ${this.escapeHtml(err.message)}</div>`;
        }
    },

    renderEscrowCard(e) {
        const badgeClass = {
            'Created': 'bg-secondary', 'Funded': 'bg-primary', 'Shipped': 'bg-warning text-dark',
            'Completed': 'bg-success', 'Disputed': 'bg-danger', 'Resolved': 'bg-info', 'Cancelled': 'bg-dark'
        }[e.state] || 'bg-secondary';

        const safeState = this.escapeHtml(e.state);
        const safeDescription = this.escapeHtml(this.truncate(e.description, 80));
        const safeAmount = this.escapeHtml(e.amount);
        const safeBuyer = this.escapeHtml(this.shortAddr(e.buyer));
        const safeSeller = this.escapeHtml(this.shortAddr(e.seller));
        const safeArbiter = this.escapeHtml(this.shortAddr(e.arbiter));
        const safeId = parseInt(e.id);

        const roleLabels = [];
        if (this.walletAddress) {
            if (e.buyer?.toLowerCase() === this.walletAddress) roleLabels.push('<span class="badge bg-primary bg-opacity-25 text-primary">Buyer</span>');
            if (e.seller?.toLowerCase() === this.walletAddress) roleLabels.push('<span class="badge bg-success bg-opacity-25 text-success">Seller</span>');
            if (e.arbiter?.toLowerCase() === this.walletAddress) roleLabels.push('<span class="badge bg-info bg-opacity-25 text-info">Arbiter</span>');
        }

        return `
            <a href="/Escrow/Detail/${safeId}" class="escrow-card">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <div class="d-flex align-items-center gap-2 mb-1">
                            <strong>Escrow #${safeId}</strong>
                            <span class="badge ${badgeClass}">${safeState}</span>
                            ${roleLabels.join(' ')}
                        </div>
                        <div class="text-muted small">${safeDescription}</div>
                    </div>
                    <div class="text-end">
                        <div class="fw-bold">${safeAmount} ETH</div>
                        <div class="text-muted small">${new Date(e.createdAt).toLocaleDateString()}</div>
                    </div>
                </div>
                <div class="d-flex gap-3 mt-2 small text-muted">
                    <span>Buyer: ${safeBuyer}</span>
                    <span>Seller: ${safeSeller}</span>
                    <span>Arbiter: ${safeArbiter}</span>
                </div>
            </a>`;
    },

    // ── Detail Page ──────────────────────────────────────
    initDetail() {
        const card = document.getElementById('actions-card');
        if (!card) return;

        this.detailEscrowId = parseInt(card.dataset.escrowId);
        this.detailState = card.dataset.state;
        this.detailBuyer = card.dataset.buyer;
        this.detailSeller = card.dataset.seller;
        this.detailArbiter = card.dataset.arbiter;
        this.currentRole = 'buyer';
        this.currentRoleAddress = this.detailBuyer;

        // Role switcher buttons
        document.querySelectorAll('.role-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelectorAll('.role-btn').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                this.currentRole = btn.dataset.role;
                this.currentRoleAddress = btn.dataset.address;
                this.renderActions();
            });
        });

        // Resolve slider
        const slider = document.getElementById('resolve-percent');
        if (slider) {
            slider.addEventListener('input', () => {
                document.getElementById('resolve-buyer-pct').textContent = slider.value;
                document.getElementById('resolve-seller-pct').textContent = 100 - parseInt(slider.value);
            });
        }

        this.renderActions();
    },

    renderActions() {
        const container = document.getElementById('action-buttons');
        if (!container) return;

        const state = this.detailState;
        const role = this.currentRole;
        const safeRole = this.escapeHtml(role);
        const safeState = this.escapeHtml(state);
        const buttons = [];

        // Confirm Shipment — seller, state=Funded
        if (role === 'seller' && state === 'Funded') {
            buttons.push(`<button class="btn btn-warning me-2 mb-2" onclick="App.doAction('ship')">Confirm Shipment</button>`);
        }

        // Confirm Receipt — buyer, state=Shipped
        if (role === 'buyer' && state === 'Shipped') {
            buttons.push(`<button class="btn btn-success me-2 mb-2" onclick="App.doAction('confirm')">Confirm Receipt</button>`);
        }

        // Raise Dispute — buyer or seller, state=Funded or Shipped
        if ((role === 'buyer' || role === 'seller') && (state === 'Funded' || state === 'Shipped')) {
            buttons.push(`<button class="btn btn-outline-danger me-2 mb-2" data-bs-toggle="collapse" data-bs-target="#dispute-input">Raise Dispute</button>`);
        }

        // Resolve Dispute — arbiter, state=Disputed
        if (role === 'arbiter' && state === 'Disputed') {
            buttons.push(`<button class="btn btn-info me-2 mb-2" data-bs-toggle="collapse" data-bs-target="#resolve-input">Resolve Dispute</button>`);
        }

        // Cancel — buyer, state=Funded
        if (role === 'buyer' && state === 'Funded') {
            buttons.push(`<button class="btn btn-outline-secondary me-2 mb-2" onclick="App.doAction('cancel')">Cancel Escrow</button>`);
        }

        if (buttons.length === 0) {
            container.innerHTML = `<p class="text-muted mb-0">No actions available for <strong>${safeRole}</strong> in state <strong>${safeState}</strong>.</p>`;
        } else {
            container.innerHTML = buttons.join('');
        }

        // Wire up dispute submit
        const disputeBtn = document.getElementById('btn-submit-dispute');
        if (disputeBtn) {
            disputeBtn.onclick = () => this.doDispute();
        }

        // Wire up resolve submit
        const resolveBtn = document.getElementById('btn-submit-resolve');
        if (resolveBtn) {
            resolveBtn.onclick = () => this.doResolve();
        }
    },

    async doAction(action) {
        try {
            const res = await fetch(`/api/escrow/${this.detailEscrowId}/${action}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ address: this.currentRoleAddress })
            });

            if (!res.ok) {
                const err = await res.json();
                throw new Error(err.error || `Action ${action} failed`);
            }

            this.showToast(`Action "${action}" executed successfully!`, 'success');
            setTimeout(() => location.reload(), 500);
        } catch (err) {
            this.showToast(err.message, 'danger');
        }
    },

    async doDispute() {
        const reason = document.getElementById('dispute-reason').value.trim();
        if (!reason) {
            this.showToast('Please enter a reason for the dispute', 'warning');
            return;
        }
        try {
            const res = await fetch(`/api/escrow/${this.detailEscrowId}/dispute`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ address: this.currentRoleAddress, reason })
            });

            if (!res.ok) {
                const err = await res.json();
                throw new Error(err.error || 'Dispute failed');
            }

            this.showToast('Dispute raised successfully', 'warning');
            setTimeout(() => location.reload(), 500);
        } catch (err) {
            this.showToast(err.message, 'danger');
        }
    },

    async doResolve() {
        const buyerPercent = parseInt(document.getElementById('resolve-percent').value);
        try {
            const res = await fetch(`/api/escrow/${this.detailEscrowId}/resolve`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ address: this.currentRoleAddress, buyerPercent })
            });

            if (!res.ok) {
                const err = await res.json();
                throw new Error(err.error || 'Resolution failed');
            }

            this.showToast('Dispute resolved successfully', 'info');
            setTimeout(() => location.reload(), 500);
        } catch (err) {
            this.showToast(err.message, 'danger');
        }
    },

    // ── Utilities ────────────────────────────────────────
    shortAddr(addr) {
        if (!addr || addr.length < 12) return addr || '???';
        return addr.slice(0, 6) + '...' + addr.slice(-4);
    },

    truncate(str, max) {
        if (!str) return '';
        return str.length > max ? str.slice(0, max) + '...' : str;
    },

    showToast(message, type = 'info') {
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        const bgClass = {
            'success': 'bg-success', 'danger': 'bg-danger', 'warning': 'bg-warning text-dark', 'info': 'bg-info'
        }[type] || 'bg-secondary';

        const toast = document.createElement('div');
        toast.className = `alert ${bgClass.replace('bg-', 'alert-')} py-2 px-3 mb-2 shadow`;
        toast.style.cssText = 'min-width: 250px; animation: fadeIn 0.3s;';
        toast.textContent = message;
        container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transition = 'opacity 0.3s';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }
};

// ── Boot ─────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => App.init());
