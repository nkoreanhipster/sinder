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
    this.element.style = "transition: width 250ms, height 250ms, background-color 250ms, transform 250ms;"

    this.hide = () => this.element.classList.remove('show')
    this.show = () => this.element.classList.add('show')
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