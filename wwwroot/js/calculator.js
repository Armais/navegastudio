var currentMode = 'RiskCalculator';
var i18n = window.i18n || {};

document.getElementById('calculateBtn').addEventListener('click', calculate);

document.querySelectorAll('input').forEach(function (input) {
    input.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') calculate();
    });
});

// Set initial mode description
document.getElementById('modeDescription').textContent = i18n.riskCalcDesc || 'Enter your position size to calculate the risk percentage for your account.';

function switchMode(mode) {
    currentMode = mode;

    var riskCalcBtn = document.getElementById('modeRiskCalc');
    var posSizerBtn = document.getElementById('modePosSizer');
    var positionSizeGroup = document.getElementById('positionSizeGroup');
    var riskPercentGroup = document.getElementById('riskPercentGroup');
    var modeDescription = document.getElementById('modeDescription');

    if (mode === 'RiskCalculator') {
        riskCalcBtn.classList.add('active');
        posSizerBtn.classList.remove('active');
        positionSizeGroup.classList.remove('d-none');
        riskPercentGroup.classList.add('d-none');
        modeDescription.textContent = i18n.riskCalcDesc || 'Enter your position size to calculate the risk percentage for your account.';
    } else {
        riskCalcBtn.classList.remove('active');
        posSizerBtn.classList.add('active');
        positionSizeGroup.classList.add('d-none');
        riskPercentGroup.classList.remove('d-none');
        modeDescription.textContent = i18n.posSizerDesc || 'Enter your desired risk percentage to calculate the optimal position size.';
    }

    document.getElementById('resultsPanel').style.display = 'none';
    document.getElementById('emptyState').style.display = 'block';
    document.getElementById('errorAlert').classList.add('d-none');
}

function calculate() {
    var errorAlert = document.getElementById('errorAlert');
    errorAlert.classList.add('d-none');

    var input = {
        mode: currentMode,
        accountCapital: parseFloat(document.getElementById('accountCapital').value),
        entryPrice: parseFloat(document.getElementById('entryPrice').value),
        stopLoss: parseFloat(document.getElementById('stopLoss').value),
        takeProfit: document.getElementById('takeProfit').value ? parseFloat(document.getElementById('takeProfit').value) : null,
        commissionPercent: parseFloat(document.getElementById('commissionPercent').value) || 0,
        tradeDirection: document.getElementById('tradeDirection').value
    };

    if (currentMode === 'RiskCalculator') {
        input.positionSize = parseFloat(document.getElementById('positionSize').value);
        if (!input.positionSize) {
            showError(i18n.errPositionSize || 'Please fill in Position Size.');
            return;
        }
    } else {
        input.riskPercent = parseFloat(document.getElementById('riskPercent').value);
        if (!input.riskPercent) {
            showError(i18n.errRiskPercent || 'Please fill in Risk Percentage.');
            return;
        }
    }

    if (!input.accountCapital || !input.entryPrice || !input.stopLoss) {
        showError(i18n.errRequiredFields || 'Please fill in Account Capital, Entry Price, and Stop Loss.');
        return;
    }

    fetch('/api/risk/calculate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(input)
    })
    .then(function (response) {
        if (!response.ok) {
            return response.json().then(function (err) {
                throw new Error(err.error || err.title || 'Calculation failed');
            });
        }
        return response.json();
    })
    .then(function (result) {
        displayResults(result);
    })
    .catch(function (err) {
        showError(err.message);
    });
}

function showError(message) {
    var alert = document.getElementById('errorAlert');
    alert.textContent = message;
    alert.classList.remove('d-none');
}

function formatCurrency(value) {
    return '$' + Number(value).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function displayResults(r) {
    document.getElementById('emptyState').style.display = 'none';
    document.getElementById('resultsPanel').style.display = 'block';

    document.getElementById('resPositionSize').textContent = Math.floor(r.positionSize).toLocaleString();
    document.getElementById('resPositionValue').textContent = formatCurrency(r.positionValue);

    document.getElementById('resRiskPercent').textContent = r.riskPercent.toFixed(2) + '%';
    document.getElementById('resRiskAmount').textContent = formatCurrency(r.riskAmount);
    document.getElementById('resStopDistance').textContent = '$' + r.stopLossDistance.toFixed(4) + ' (' + r.stopLossPercent.toFixed(2) + '%)';
    document.getElementById('resMaxLoss').textContent = formatCurrency(r.maxLoss);
    document.getElementById('resCommission').textContent = formatCurrency(r.commissionCost);
    document.getElementById('resNetMaxLoss').textContent = formatCurrency(r.netMaxLoss);
    document.getElementById('resPortfolioRisk').textContent = r.portfolioRiskPercent.toFixed(2) + '%';
    document.getElementById('resLeverage').textContent = r.leverage.toFixed(2) + 'x';

    var tpSection = document.getElementById('tpSection');
    if (r.riskRewardRatio !== null) {
        tpSection.style.display = 'block';
        document.getElementById('resTpDistance').textContent = '$' + r.takeProfitDistance.toFixed(4) + ' (' + r.takeProfitPercent.toFixed(2) + '%)';
        document.getElementById('resPotentialProfit').textContent = formatCurrency(r.potentialProfit);
        document.getElementById('resNetProfit').textContent = formatCurrency(r.netPotentialProfit);
        document.getElementById('resRR').textContent = '1:' + r.riskRewardRatio.toFixed(2);
        document.getElementById('resKelly').textContent = r.kellyPercent.toFixed(2) + '% (' + Math.floor(r.kellyPositionSize) + ' units)';
    } else {
        tpSection.style.display = 'none';
    }

    var riskPct = r.portfolioRiskPercent;
    var riskBar = document.getElementById('riskBar');
    var barWidth = Math.min(riskPct * 10, 100);
    riskBar.style.width = barWidth + '%';
    riskBar.textContent = riskPct.toFixed(1) + '%';

    if (riskPct <= 1) {
        riskBar.className = 'progress-bar bg-success';
        document.getElementById('riskAdvice').textContent = i18n.riskConservative || 'Conservative risk level. Well within safe limits.';
    } else if (riskPct <= 2) {
        riskBar.className = 'progress-bar bg-info';
        document.getElementById('riskAdvice').textContent = i18n.riskStandard || 'Standard risk level. Within recommended range (1-2%).';
    } else if (riskPct <= 5) {
        riskBar.className = 'progress-bar bg-warning';
        document.getElementById('riskAdvice').textContent = i18n.riskElevated || 'Elevated risk. Consider reducing position size.';
    } else {
        riskBar.className = 'progress-bar bg-danger';
        document.getElementById('riskAdvice').textContent = i18n.riskHigh || 'High risk! This exceeds recommended levels. Reduce position size or widen stop loss.';
    }
}
