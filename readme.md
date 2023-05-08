# Unity Model Task Viewer - Identify Errors #

(c) 2023 May 4 Vienna Ly  
<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/" target="_blank"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a></a>

## App description ##
Build published by **MTV_identifyErrors** Unity project Engine v 2022.1.7f1.  The model viewer task is to identify errors in the scene:

- Objects in scene that are errors will be named with suffix `- ERROR`.

Web handler will:

- Handle scene loading
- Read HTML table for optional description overrides for items specified in table
- Set up the Checklist self-quiz component:
  - Creates list from scene items
  - Shows answers and corresponding descriptions with associated scene items
