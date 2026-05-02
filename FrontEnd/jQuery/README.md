# jQuery (legacy)

> For new code, prefer plain DOM (`document.querySelector`, `fetch`). This folder is reference for maintaining legacy apps.

## Native equivalents (the migration map)

| jQuery | Modern DOM / fetch |
|---|---|
| `$('.x')` | `document.querySelector('.x')` (or `querySelectorAll`) |
| `$(el).hide()` | `el.style.display = 'none'` |
| `$(el).show()` | `el.style.display = ''` |
| `$(el).addClass('x')` | `el.classList.add('x')` |
| `$(el).text(x)` | `el.textContent = x` |
| `$(el).html(x)` | `el.innerHTML = x` (DANGER: XSS unless sanitized) |
| `$.ajax({...})` | `fetch(url, {...})` |
| `$.getJSON(url)` | `fetch(url).then(r => r.json())` |
| `$(document).on('click', '.x', fn)` | event delegation: `document.addEventListener('click', e => { if (e.target.matches('.x')) fn(e); })` |
| `$(el).fadeIn()` | CSS transition + class toggle, or Web Animations API |

## Common Pitfalls (in legacy)

- `$.ajax` synchronous — kills the page; never do this
- `$(el).html(userInput)` — XSS vector
- Multiple jQuery versions loaded — silent breakage
- Plugins that target `$` — ensure jQuery is exposed if you use `noConflict`

## Phase-out strategy

1. Replace selectors and DOM ops one file at a time
2. Replace `$.ajax` with `fetch`
3. Wait until the last plugin is gone
4. Drop the script tag

## See also

- [../JavaScript](../JavaScript/) · [../../Modernization/WebFormsToBlazor](../../Modernization/WebFormsToBlazor/)
