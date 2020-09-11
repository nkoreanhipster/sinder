// Fjantig notifikations-kontroller
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
})()