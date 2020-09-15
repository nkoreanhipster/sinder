// Stuff events in an orderly queue
const Queue = (function () {
    this.queue = []
    this.enqueue = (...args) => [...args].map(item => this.queue.push(item))
    this.dequeue = () => this.queue.shift()
    this.clear = () => this.queue = []
    return this
})();

// -//- but with namespaces
const NamedspacedQueue = (function () {
    this.queue = []
    var queueItem = (ns, item) => {
        return {
            key: ns,
            item: item
        }
    }
    var getItemWithNamespace = (ns) => this.queue.filter(item => item.key === ns).shift()
    this.enqueue = (ns, ...args) => [...args].map(item => this.queue.push(queueItem(ns, item)))
    this.dequeue = (ns) => getItemWithNamespace(ns) || null
    this.getByNs = (ns) => this.queue.filter(item => item.key === ns)
    this.clear = () => this.queue = []
    return this
})();

// Fjantig notifikations-kontroller
var Cheesebox = (function () {
    this.element = document.querySelector('#toast_box')
    this.body = document.querySelector('#toast_box > .toast-body')
    this.element.style = "transition: width 250ms, height 250ms, background-color 250ms, transform 250ms;"



    this.hide = () => this.element.classList.remove('show')
    this.show = (text) => {
        this.body.innerText = text
        this.element.classList.add('show')
    }
    this.element.querySelector('button[data-dismiss]').addEventListener('click', (ev) => {
        this.hide()
    })



    return this
})();

// todo; namespaces and stuff
var localStorageExtender = (function () {
    this.load = (key) => window.localStorage.getItem(key)
    this.save = (key, value) => window.localStorage.setItem(key, value)
    this.delete = (key) => window.localStorage.removeItem(key)
    return this
})();


// Set active browser cookie
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; samesite=None" + "; path=/; Secure";
}

// Retrieve active browser cookie
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

// Delete specified cookie
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

//Check if email is valid
function isValidEmail(email) {
    const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function isValidDate(d) {
    return d instanceof Date && !isNaN(d);
}

// Catch all elements with attribute 'data-toggle'
var toggableElements = [...document.querySelectorAll('*[data-toggle]')]
    ; toggableElements.map(el => {

        el.addEventListener('click', (ev) => {
            var toggleType = el.getAttribute('data-toggle')
            var targetName = el.getAttribute('data-target')
            var targetEle = document.querySelector(targetName) || null

            if (toggleType === 'collapse') {
                targetEle.classList.toggle('collapse')
            }
            if (toggleType === 'dropdown') {
                targetEle.classList.toggle('show')
            }
        })

    })



// Reactive stuff set up

// Add info to footer, todo; set somewhere else
if (getCookie('token') !== null) {
    var footerContainer = document.querySelector('footer > .container')
    var span = document.createElement('span')
    span.style.overflow = "hidden"
    span.style.whiteSpace = "nowrap"
    span.style.display = "block"
    span.style.fontSize = "0.8rem"
    span.innerHTML = `token=${getCookie('token')}`;
    footerContainer.append(span)
}

// Log out listener
var logoutButton = document.querySelector('ul.navbar-nav > .nav-item > a.nav-link[href="/logout"]')
// Remove token and reload home page
logoutButton.addEventListener('click', (ev) => {
    ev.preventDefault()
    eraseCookie('token')
    window.location.replace('/home')
})

// Listeners for menu buttons
var currentUrl = new URL(window.location.href)
var navButtons = [...document.querySelectorAll('ul.navbar-nav > .nav-item')];
//var currentPageMarker = document.querySelector('.nav-link > span.sr-only')
navButtons.map(navBtn => {
    // Switch navbutton page marker
    if (navBtn.querySelector('a').href === currentUrl.href)
        navBtn.classList.add('active')
    else
        navBtn.classList.remove('active')
})

// Remove all open drop-down toggles on window resize
// To fix the UI-bug with a drop-down remaining open if window is resized
NamedspacedQueue.enqueue('onWindowResize', (ev) => {
    var dropdownElements = toggableElements.filter(el => el.hasAttribute('data-toggle') && el.getAttribute('data-toggle') === 'dropdown')
    var targetElements = dropdownElements.map(el => document.querySelector(el.getAttribute('data-target')))
    targetElements.map(el => el.classList.remove('show'))
})

window.addEventListener('resize', (ev) => {
    NamedspacedQueue.getByNs('onWindowResize').map(func => {
        if (func.item && typeof func.item === 'function')
            func.item()
    })
})