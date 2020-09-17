﻿// Stuff events in an orderly queue
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

var Successbox = (function () {
    this.element = document.querySelector('#success_box')
    this.body = document.querySelector('#success_box > .toast-body')
    this.element.style = "transition: width 250ms, height 250ms, background-color 250ms, transform 100ms;"

    this.hide = () => this.element.classList.remove('show')
    this.show = (text) => {
        this.element.style = "transform: translateY(-100px); transition: all 1500ms; "
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
            ev.preventDefault()
            var toggleType = el.getAttribute('data-toggle')
            var targetName = el.getAttribute('data-target')
            var targetEle = document.querySelector(targetName) || null

            console.log(toggleType, targetName, targetEle)

            if (toggleType === 'collapse') {
                targetEle.classList.toggle('collapse')
            }
            if (toggleType === 'dropdown') {
                targetEle.classList.toggle('show')
            }
            if (toggleType == 'modal') {
                console.log('closed')
                targetEle.classList.toggle('show')
                targetEle.classList.toggle('open')
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

// File upload listener
(function fileUploadListener() {

    var dropArea = document.querySelector('.droparea')
    var imageFileInput = document.querySelector('#image_file_input')
    var progressBar = document.querySelector('#upload_progressbar')
    var progressCounter = document.querySelector('#progress_counter')
    var uploadedImage = document.querySelector('#uploaded_image')
    var imagePreviewList = document.querySelector('#image_preview_list')
    

    const initializeProgress = function (numFiles) {
        progressBar.value = 0
        uploadProgress = []

        for (let i = numFiles; i > 0; i--) {
            uploadProgress.push(0)
        }
    }

    const updateProgress = function (fileNumber, percent) {
        uploadProgress[fileNumber] = percent
        let total = uploadProgress.reduce((tot, curr) => tot + curr, 0) / uploadProgress.length
        console.debug('update', fileNumber, percent, total)
        progressCounter.textContent = `${percent}%`
        progressBar.value = total
    }

    const uploadFile = function (file, i) {
        var url = "/api/file"
        var xhr = new XMLHttpRequest()
        var formData = new FormData()
        xhr.open('POST', url, true)
        //xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest')
        //xhr.setRequestHeader('Access-Control-Allow-Origin', 'localhost')
        //xhr.setRequestHeader('Content-Type', 'multipart/form-data')

        // Update progress (can be used to show progress indicator)
        xhr.upload.addEventListener("progress", function (e) {
            updateProgress(i, (e.loaded * 100.0 / e.total) || 100)
        })

        xhr.addEventListener('readystatechange', function (ev) {
            if (xhr.readyState == 4 && xhr.status == 200) {
                updateProgress(i, 100) // <- Add this
                if (ev.target.response) {
                    var json = JSON.parse(ev.target.response)
                    console.log('UPLOADED!', { json: json })
                    uploadedImage.src = json.Data.shift().Url;
                }
                else {
                    console.error('Files are wrong')
                }
            }
            else if (xhr.readyState == 4 && xhr.status != 200) {
                console.error('Something wung')
            }
        })

        xhr.addEventListener('error', function (ev) {
            console.error({ error: ev.error })
        })
        // Cover for existing file
        //formData.append('cover', false)
        //formData.append('upload_preset', 'ijfgiouahfbnuivboaefh')
        formData.append('file', file)
        console.log({ file: file, i: i, formData: formData })
        xhr.send(formData)
    }

    const highlight = function (ev) {
        dropArea.classList.add('bg-info')
    }

    const unhighlight = function (ev) {
        dropArea.classList.remove('bg-info')
    }

    const preventDefaults = function (ev) {
        ev.preventDefault()
        ev.stopPropagation()
    }

    const readURL = file => {
        return new Promise((res, rej) => {
            const reader = new FileReader();
            reader.onload = e => res(e.target.result);
            reader.onerror = e => rej(e);
            reader.readAsDataURL(file);
        });
    };

    const createImageElement = (src) => {
        var img = document.createElement('img')
        img.classList.add('img')
        img.src = src
        imagePreviewList.appendChild(img)
    }

    const handleFiles = async(event) => {

        var urls = []

        for (let i = 0; i < event.target.files.length; i++) {
            const file = event.target.files[i]; 
            await readURL(file)
                .then(url => urls.push(url))

        }

        urls.map(url => createImageElement(url))
    }

    const handleDropFiles = async (event) => {
        files = [...event];
        initializeProgress(files.length)
        files.map((file, iterator) => uploadFile(file, iterator))
    }

    const handleDrop = (ev) => {
        var dt = ev.dataTransfer
        var files = dt.files
        handleDropFiles(files)
    }

    // Listens for change stuff
    imageFileInput.addEventListener('change', (ev) => handleFiles(ev))

        // Prevent drag default behaviour
        ;['dragenter', 'dragover', 'dragleave', 'drop']
            .map(eventName => {
                window.addEventListener(eventName, preventDefaults, false)
            })
        // Highlight drop area when item is dragged over it
        ;['dragenter', 'dragover'].forEach(eventName => {
            dropArea.addEventListener(eventName, highlight, false)
        })
        ;['dragleave', 'drop'].forEach(eventName => {
            dropArea.addEventListener(eventName, unhighlight, false)
        })

    // Handle dropped files
    dropArea.addEventListener('drop', handleDrop, false)

})()


// Listeners for menu buttons
var currentUrl = new URL(window.location.href)
var navButtons = [...document.querySelectorAll('ul.navbar-nav > .nav-item')];

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

// Watch for some stuff on window click
NamedspacedQueue.enqueue('onWindowClick', (ev) => {
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
