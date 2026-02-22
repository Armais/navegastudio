// NavegaStudio - Global JavaScript
document.addEventListener('DOMContentLoaded', function () {

    // --- AOS (Animate on Scroll) Init ---
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            easing: 'ease-out-cubic',
            once: true,
            offset: 80,
            disable: window.innerWidth < 768
        });
    }

    // --- Navbar Scroll Effect ---
    var navbar = document.querySelector('.navbar-ns');
    if (navbar) {
        var onScroll = function () {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll(); // run once on load
    }

    // --- Smooth Scroll for Anchor Links ---
    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            var target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });
});

// --- CountUp Observer (global, called from Home page) ---
function initCountUpObserver() {
    if (typeof countUp === 'undefined' && typeof CountUp === 'undefined') return;

    var CountUpClass = (typeof CountUp !== 'undefined') ? CountUp : countUp.CountUp;
    var elements = document.querySelectorAll('.countup');
    if (!elements.length) return;

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                var el = entry.target;
                var endVal = parseFloat(el.getAttribute('data-target'));
                var suffix = el.getAttribute('data-suffix') || '';
                var prefix = el.getAttribute('data-prefix') || '';
                var decimals = el.getAttribute('data-decimals') ? parseInt(el.getAttribute('data-decimals')) : 0;

                var counter = new CountUpClass(el, endVal, {
                    duration: 2.5,
                    suffix: suffix,
                    prefix: prefix,
                    decimalPlaces: decimals,
                    useGrouping: true
                });
                if (!counter.error) {
                    counter.start();
                }
                observer.unobserve(el);
            }
        });
    }, { threshold: 0.3 });

    elements.forEach(function (el) { observer.observe(el); });
}
