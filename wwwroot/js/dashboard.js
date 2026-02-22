const i18n = window.i18n || {};

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/pricehub")
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

let priceChart = null;
const priceHistory = {};

function formatPrice(price) {
    if (price == null || isNaN(price)) return '-';
    if (price >= 100)   return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    if (price >= 1)     return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 4 });
    if (price >= 0.01)  return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 4, maximumFractionDigits: 4 });
    if (price >= 0.001) return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 5, maximumFractionDigits: 5 });
    return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 6, maximumFractionDigits: 6 });
}

function formatSpreadDollar(price) {
    if (price == null || isNaN(price)) return '-';
    if (price >= 1)      return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    if (price >= 0.01)   return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 4, maximumFractionDigits: 4 });
    return '$' + price.toLocaleString('en-US', { minimumFractionDigits: 6, maximumFractionDigits: 6 });
}

function formatVolume(vol) {
    if (vol == null || isNaN(vol) || vol === 0) return '-';
    if (vol >= 1e9)  return '$' + (vol / 1e9).toFixed(2) + 'B';
    if (vol >= 1e6)  return '$' + (vol / 1e6).toFixed(2) + 'M';
    if (vol >= 1e3)  return '$' + (vol / 1e3).toFixed(1) + 'K';
    return '$' + vol.toFixed(2);
}

function getChangeClass(change) {
    return change >= 0 ? 'text-up' : 'text-down';
}

function getChangeArrow(change) {
    return change >= 0 ? '&#9650;' : '&#9660;';
}

function updatePriceTable(snapshots) {
    const tbody = document.getElementById('priceTable');
    let html = '';

    snapshots.forEach(function (s) {
        const binance = s.exchanges.find(function (e) { return e.exchange === 'Binance'; });
        const coinbase = s.exchanges.find(function (e) { return e.exchange === 'Coinbase'; });
        const kraken = s.exchanges.find(function (e) { return e.exchange === 'Kraken'; });
        const change = (binance && binance.change24hPercent) || (coinbase && coinbase.change24hPercent) || 0;
        const changeClass = getChangeClass(change);

        html += '<tr>' +
            '<td><strong>' + s.symbol + '</strong>/USDT</td>' +
            '<td>' + (binance ? formatPrice(binance.price) : '<span class="text-muted">N/A</span>') + '</td>' +
            '<td>' + (coinbase ? formatPrice(coinbase.price) : '<span class="text-muted">N/A</span>') + '</td>' +
            '<td>' + (kraken ? formatPrice(kraken.price) : '<span class="text-muted">N/A</span>') + '</td>' +
            '<td>' + (s.spreadPercent != null ? s.spreadPercent.toFixed(2) + '%' : '-') + '</td>' +
            '<td class="' + changeClass + '">' + getChangeArrow(change) + ' ' + Math.abs(change).toFixed(2) + '%</td>' +
            '</tr>';

        if (!priceHistory[s.symbol]) priceHistory[s.symbol] = [];
        const avgPrice = s.exchanges.reduce(function (sum, e) { return sum + e.price; }, 0) / s.exchanges.length;
        priceHistory[s.symbol].push({ time: new Date(), price: avgPrice });
        if (priceHistory[s.symbol].length > 20) priceHistory[s.symbol].shift();
    });

    tbody.innerHTML = html;
    document.getElementById('lastUpdate').textContent = new Date().toLocaleTimeString();
    updateChart(snapshots);
}

function updateArbitrage(opportunities) {
    const section = document.getElementById('arbitrageSection');
    const cards = document.getElementById('arbitrageCards');

    if (opportunities.length === 0) {
        section.style.display = 'none';
        return;
    }

    section.style.display = 'block';
    let html = '';
    opportunities.forEach(function (opp) {
        html += '<div class="col-md-4">' +
            '<div class="card arb-alert">' +
            '<div class="card-body">' +
            '<h6><strong>' + opp.symbol + '</strong></h6>' +
            '<p class="mb-1">' + (i18n.buyOn || 'Buy on') + ' <span class="badge bg-success badge-exchange">' + opp.buyExchange + '</span> at ' + formatPrice(opp.buyPrice) + '</p>' +
            '<p class="mb-1">' + (i18n.sellOn || 'Sell on') + ' <span class="badge bg-danger badge-exchange">' + opp.sellExchange + '</span> at ' + formatPrice(opp.sellPrice) + '</p>' +
            '<p class="mb-0 text-warning fw-bold">' + (i18n.spread || 'Spread') + ': ' + opp.spreadPercent.toFixed(2) + '% (' + formatSpreadDollar(opp.spreadAmount) + ')</p>' +
            '</div></div></div>';
    });
    cards.innerHTML = html;
}

function updateChart(snapshots) {
    var ctx = document.getElementById('priceChart');
    var chartSymbols = ['BTC', 'ETH', 'SOL'];
    var colors = ['#58a6ff', '#f0883e', '#3fb950'];

    var datasets = snapshots
        .filter(function (s) { return chartSymbols.indexOf(s.symbol) !== -1; })
        .map(function (s, i) {
            var history = priceHistory[s.symbol] || [];
            return {
                label: s.symbol,
                data: history.map(function (h) { return h.price; }),
                borderColor: colors[i % 3],
                tension: 0.3,
                pointRadius: 2,
                borderWidth: 2,
                fill: false
            };
        });

    var btcHistory = priceHistory['BTC'] || [];
    var labels = btcHistory.map(function (h) { return h.time.toLocaleTimeString(); });

    if (priceChart) {
        priceChart.data.labels = labels;
        priceChart.data.datasets = datasets;
        priceChart.update('none');
    } else {
        priceChart = new Chart(ctx, {
            type: 'line',
            data: { labels: labels, datasets: datasets },
            options: {
                responsive: true,
                plugins: { legend: { labels: { color: '#c9d1d9' } } },
                scales: {
                    x: { ticks: { color: '#8b949e', maxTicksLimit: 10 }, grid: { color: '#21262d' } },
                    y: { ticks: { color: '#8b949e', callback: function (v) { return '$' + v.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); } }, grid: { color: '#21262d' } }
                }
            }
        });
    }
}

connection.on("ReceivePrices", updatePriceTable);
connection.on("ReceiveArbitrage", updateArbitrage);

connection.onreconnecting(function () {
    document.getElementById('connectionStatus').className = 'text-warning pulse';
    document.getElementById('connectionStatus').textContent = i18n.reconnecting || 'Reconnecting...';
});

connection.onreconnected(function () {
    document.getElementById('connectionStatus').className = 'text-up';
    document.getElementById('connectionStatus').textContent = i18n.connected || 'Connected';
});

connection.onclose(function () {
    document.getElementById('connectionStatus').className = 'text-down';
    document.getElementById('connectionStatus').textContent = i18n.disconnected || 'Disconnected';
});

function fetchViaRest() {
    fetch('/api/prices')
        .then(function (res) { return res.json(); })
        .then(function (data) { updatePriceTable(data); })
        .catch(function (e) { console.error('REST prices failed:', e); });

    fetch('/api/arbitrage')
        .then(function (res) { return res.json(); })
        .then(function (data) { updateArbitrage(data); })
        .catch(function (e) { console.error('REST arbitrage failed:', e); });

    setTimeout(fetchViaRest, 15000);
}

// Set initial loading message
var loadingEl = document.getElementById('loadingMsg');
if (loadingEl) loadingEl.textContent = i18n.loadingPrices || 'Loading prices...';

// Set initial connection status
document.getElementById('connectionStatus').textContent = i18n.connecting || 'Connecting...';

async function start() {
    try {
        await connection.start();
        document.getElementById('connectionStatus').className = 'text-up';
        document.getElementById('connectionStatus').textContent = i18n.connected || 'Connected';
        await connection.invoke("RequestPrices");
        await connection.invoke("RequestArbitrage");
    } catch (err) {
        document.getElementById('connectionStatus').className = 'text-down';
        document.getElementById('connectionStatus').textContent = i18n.connectionFailed || 'Connection failed - using REST';
        console.error('SignalR error:', err);
        fetchViaRest();
    }
}

start();
