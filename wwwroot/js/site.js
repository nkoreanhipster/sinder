const Eggplant = (function () {
    this.element = document.querySelector('#eggplant')
    return this
})()


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
var Successbox = (function () {
    var self = {}

    self.element = document.querySelector('#success_box')
    self.body = self.element.querySelector('.toast-body')
    self.title = self.element.querySelector('.toast-header > strong')

    // Go away
    self.hide = () => self.element.classList.remove('show')

    self.setBackgroundColor = (color) => self.element.style.backgroundColor = color || '#fff'

    self.setColor = (color) => self.element.style.color = color || '#fff'

    self.setTitle = (text) => self.title.textContent = text

    self.clearText = () => {
        // Clear text and nodes
        while (self.body.firstChild) {
            self.body.removeChild(self.body.lastChild);
        }
    }

    self.setText = (text) => {
        var p = document.createElement('p')
        //p.classList.add('display-1')
        p.textContent = text
        self.body.append(p)
    }

    // Flash annoyingly
    self.flash = (durationMs = 1000) => {
        var start = new Date().getTime();
        var counter = 0;
        var handle = window.setInterval(() => {
            var flashingColors = ['blue', 'red', 'white', 'black']
            var n = counter % 2
            self.setBackgroundColor(flashingColors[n])
            self.setColor(flashingColors[n + 2])

            // Break loop if specified milliseconds has passed since start
            var currentMs = new Date().getTime()

            if (currentMs > (start + durationMs)) {
                window.clearInterval(handle)
            }

            counter++
        }, 2)
    }

    self.show = (text, title) => {
        self.setTitle(title || '')
        self.clearText()
        self.setText(text || '')
        self.element.classList.add('show')
    }
    self.element.querySelector('button[data-dismiss]').addEventListener('click', (ev) => {
        self.hide()
    })
    return self
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

            if (toggleType === 'collapse' && targetEle !== null) {
                targetEle.classList.toggle('collapse')
            }
            if (toggleType === 'dropdown' && targetEle !== null) {
                targetEle.classList.toggle('show')
            }
            if (toggleType == 'modal' && targetEle !== null) {
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
    var uploadProgress = null;

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
        progressCounter.textContent = `${percent}%`
        progressBar.value = total
    }

    const uploadFile = function (file, i, callback) {
        var url = "/api/file"
        var xhr = new XMLHttpRequest()
        var formData = new FormData()
        xhr.open('POST', url, true)

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
                    // Callback uploadedImage.src = json.Data.shift().Url;
                    callback(json.Data.shift().Url)
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

        formData.append('file', file)
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
        var parent = document.createElement('div')
        var img = document.createElement('img')
        var deleteBtn = document.createElement('a')
        var icon = document.createElement('i')

        icon.classList.add('fas', 'fa-times', 'fa-3x')
        deleteBtn.classList.add('text-danger')
        deleteBtn.href = "#"
        // Delete button clears image
        deleteBtn.addEventListener('click', (ev) => {
            ev.preventDefault()
            var c = ev.target.parentElement
            if (c.tagName === 'A')
                c = c.parentElement
            var d = c.parentElement
            d.removeChild(c)
            // Remove top image as well
            document.querySelector('#uploaded_image').src = ""
        })
        img.classList.add('is-128x128', 'take-me', 'img')
        img.src = src

        deleteBtn.append(icon)
        parent.append(img)
        parent.append(deleteBtn)
        imagePreviewList.append(parent)

        return img
    }

    const handleFiles = async (ev) => {
        const targetFiles = Array.from(ev.target.files);
        initializeProgress(ev.target.files.length)
        var urls = []

        for (let i = 0; i < ev.target.files.length; i++) {
            const file = ev.target.files[i];
            await readURL(file)
                .then(url => urls.push(url))
        }

        window.setTimeout(() => { }, 900)

        urls
            .map(url => createImageElement(url))
            .map((imgEle, index) => {
                uploadFile(targetFiles[index], index, function (imageUrl) {
                    imgEle.src = imageUrl
                })
            })

        document.querySelector('img.placeholder').src = urls.pop()
    }

    const handleDropFiles = async (ev) => {
        var files = [...ev];
        initializeProgress(files.length)

        var urls = []

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            await readURL(file)
                .then(url => urls.push(url))
        }
        urls
            .map(url => createImageElement(url))
            .map((imgEle, index) => {
                uploadFile(files[index], index, function (imageUrl) {
                    imgEle.src = imageUrl
                })
            })

        document.querySelector('img.placeholder').src = urls.pop()
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

// Events that triggers by keyboard presses
NamedspacedQueue.enqueue('onKeyDown', (ev) => {
    // Auto submit form on pressing enter
    // Search siblings via parents for a submit button
    var elHandle = document.activeElement

    if (ev.key === 'Enter' && elHandle.tagName.includes("INPUT") && elHandle.classList.contains('form-control')) {

        var submitBtnWasFound = false

        // Do max three iterations
        for (var i = 0; i < 3; i++) {
            elHandle = elHandle.parentElement
            var tempVar = elHandle.querySelector('*[type="submit"], #search_button')
            submitBtnWasFound = tempVar !== null ? true : false
            if (submitBtnWasFound) {
                elHandle = tempVar
                break;
            }

        }

        // Click on element if the button was found
        try {
            if (submitBtnWasFound)
                elHandle.click()
        }
        catch (err) { console.error(err) }
    }
})

window.addEventListener('resize', (ev) => {
    NamedspacedQueue.getByNs('onWindowResize').map(func => {
        if (func.item && typeof func.item === 'function')
            func.item(ev)
    })
})

window.addEventListener('keydown', (ev) => {
    NamedspacedQueue.getByNs('onKeyDown').map(func => {
        if (func.item && typeof func.item === 'function')
            func.item(ev)
    })
})
