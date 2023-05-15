# Unity Model Task Viewer - WebGL Builds #

(c) 2023 May 4 Vienna Ly  
<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/" target="_blank"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a></a>

## App description ##

This repo is for hosting builds and publishing to github pages for review. Each build should be in it's own subdirectory:

- [Aesculap Pan](https://vie74050.github.io/MTV_identifyErrors/AesculapPan/)

Everything is built from external resources. 

### Unity builder ###

Build published by **MTV_identifyErrors_Unity** project Engine v 2022.1.7f1.  
src: <https://github.com/vie74050/MTV_identifyErrors_Unity>

- Objects in scene that are errors will be named with suffix `- ERROR`.
- Build settings should have `Decompression Fallback` checked under `Player settings > Publishing Settings`

### Web handler ###

src: <https://github.com/vie74050/MTV_identifyErrors_web>

- Handle scene loading
- Read HTML table for optional description overrides for items specified in table
- Set up the Checklist self-quiz component:
  - Creates list from scene items
  - Shows answers and corresponding descriptions with associated scene items

## HTML Table ##

A `<table>` can be added to the built `index.html`to over ride default descriptions/prompts.  

e.g. For AesculapPan

  ```html
  <table>
      <thead>
        <tr>
          <th>Name: must match part name on model</th>
          <th>Content</th>
        </tr>
      </thead>

      <tbody>
        <tr>
          <td>Blue Filter Paper - ERROR</td>
          <td>
            <p>The filter is missing</p>
          </td>
        </tr>
        <tr>
          <td>External Sterile Indicator - ERROR</td>
          <td>
            <p>Sterile indicator is pink.</p>
          </td>
        </tr>
        <tr>
          <td>Plastic Lock - ERROR</td>
          <td>
            <p>Left plastic lock missing.</p>
          </td>
        </tr>
        <tr>
          <td>Indicator - ERROR</td>
          <td>
            <p>Indicator shows unsterile</p>
          </td>
        </tr>
        <tr>
          <td>Curved Mayo Scissors - ERROR</td>
          <td>
            <p>Dried blood on Mayo scissors.</p>
          </td>
        </tr>
      </tbody>
    </table>
  ```
After Unity Build, the `index.html` will be re-written and revert to template content. If any changes were previously made, just revert the `index.html` so the change are not pushed.
