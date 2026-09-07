#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Junta los .md del entregable en un solo PDF paginado y con indice.

    python3 06-entrega/armar-pdf.py

Requiere Google Chrome instalado. No instala nada.
"""
import base64, html, os, re, shutil, subprocess, sys, tempfile

RAIZ = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SALIDA_HTML = os.path.join(RAIZ, '07-entrega', 'documento.html')
SALIDA_PDF  = os.path.join(RAIZ, '07-entrega', 'Reto2-Documento-Sustentacion.pdf')
SALIDA_DOCX = os.path.join(RAIZ, '07-entrega', 'Reto2_Hacienda_Documento_Sustentacion_Final.docx')
CHROME = (os.environ.get('CHROME') or shutil.which('google-chrome') or
          shutil.which('chromium') or shutil.which('chromium-browser') or
          '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome')
LIBREOFFICE = shutil.which('libreoffice') or shutil.which('soffice')

# Orden del punto 5 del enunciado. Comenta una linea para dejarla fuera del PDF.
SECCIONES = [
    ('Actividad 1 · Puntos de dolor',            '01-analisis/A1_Puntos_de_Dolor.md'),
    ('Actividad 2.1 · Decisión de patrones',     '02-decision-patrones/A2_Decision_de_Patrones.md'),
    ('Actividad 2.2 · Bitácora frente a la IA',  '02-decision-patrones/A2_Bitacora_IA.md'),
    ('Actividad 3.1 · Diseño AS-IS / TO-BE',     '03-diseno/A3_DISENO_ASIS_TOBE.md'),
    ('Actividad 3.2 · Tabla de cambio estructural','03-diseno/A3_MATRIZ_CAMBIO_ESTRUCTURAL.md'),
    ('Actividad 3.3 · Fichas de los patrones',   '03-diseno/A3_FICHAS_PATRONES.md'),
    ('Actividad 4.1 · Matriz SOLID',             '04-verificacion/MATRIZ-SOLID-PATRONES.md'),
    ('Actividad 4.2 · Evidencia de comportamiento','04-verificacion/EVIDENCIA-COMPORTAMIENTO.md'),
    ('Actividad 5 · Registro de riesgos',        '05-riesgos/REGISTRO-RIESGOS.md'),
    ('Actividad 6 · Vista para el negocio',      '06-vistas/VISTA-NEGOCIO.md'),
    ('Actividad 6 · Vista para el equipo',       '06-vistas/VISTA-TECNICA.md'),
]

def en_linea(t):
    t = html.escape(t)
    t = re.sub(r'`([^`]+)`', r'<code>\1</code>', t)
    t = re.sub(r'\*\*([^*]+)\*\*', r'<strong>\1</strong>', t)
    t = re.sub(r'(?<!\w)\*([^*]+)\*(?!\w)', r'<em>\1</em>', t)
    t = re.sub(r'\[([^\]]+)\]\([^)]+\)', r'\1', t)      # enlaces: solo el texto
    return t.replace('\\|', '|')

def md_a_html(md, id_sec):
    out, i, lineas = [], 0, md.split('\n')
    while i < len(lineas):
        l = lineas[i]
        if l.startswith('```'):                          # bloque de codigo
            i += 1; buf = []
            while i < len(lineas) and not lineas[i].startswith('```'):
                buf.append(html.escape(lineas[i])); i += 1
            out.append('<pre>' + '\n'.join(buf) + '</pre>'); i += 1; continue
        if l.startswith('|'):                            # tabla
            filas = []
            while i < len(lineas) and lineas[i].startswith('|'):
                filas.append(lineas[i]); i += 1
            celdas = lambda f: [c.strip() for c in re.split(r'(?<!\\)\|', f)[1:-1]]
            cab, cuerpo = celdas(filas[0]), filas[2:]
            t = ['<table><thead><tr>'] + [f'<th>{en_linea(c)}</th>' for c in cab] + ['</tr></thead><tbody>']
            for f in cuerpo:
                t += ['<tr>'] + [f'<td>{en_linea(c)}</td>' for c in celdas(f)] + ['</tr>']
            out.append(''.join(t + ['</tbody></table>'])); continue
        m = re.match(r'^(#{1,6}) (.*)', l)
        if m:                                            # titulo
            n = len(m.group(1)) + 1                      # baja un nivel: el H1 es el de la seccion
            out.append(f'<h{min(n,6)}>{en_linea(m.group(2))}</h{min(n,6)}>'); i += 1; continue
        if l.startswith('> '):
            out.append(f'<blockquote>{en_linea(l[2:])}</blockquote>'); i += 1; continue
        if re.match(r'^[-*] ', l):                       # lista
            it = []
            while i < len(lineas) and re.match(r'^[-*] ', lineas[i]):
                it.append(f'<li>{en_linea(lineas[i][2:])}</li>'); i += 1
            out.append('<ul>' + ''.join(it) + '</ul>'); continue
        if l.strip() == '':
            i += 1; continue
        par = [l]                                        # parrafo
        i += 1
        while i < len(lineas) and lineas[i].strip() and not re.match(r'^(#|\||```|> |[-*] )', lineas[i]):
            par.append(lineas[i]); i += 1
        out.append('<p>' + en_linea(' '.join(par)) + '</p>')
    return '\n'.join(out)

def diagramas_uml():
    def png_embebido(nombre):
        ruta = os.path.join(RAIZ, '03-diseno', 'diagramas', nombre)
        with open(ruta, 'rb') as archivo:
            return base64.b64encode(archivo.read()).decode('ascii')

    return ('<figure><img width="600" style="width:600px;height:auto" src="data:image/png;base64,' + png_embebido('A3-ASIS.png') + '" '
            'alt="UML AS-IS"><figcaption>UML AS-IS</figcaption></figure>'
            '<figure><img width="600" style="width:600px;height:auto" src="data:image/png;base64,' + png_embebido('A3-TOBE.png') + '" '
            'alt="UML TO-BE"><figcaption>UML TO-BE</figcaption></figure>')

CSS = """
@page { size: A4; margin: 18mm 15mm 20mm 15mm;
        @bottom-center { content: counter(page); font-size: 9pt; } }
body { font: 8.5pt/1.25 -apple-system, 'Helvetica Neue', Arial, sans-serif; color: #111; }
h1 { font-size: 15pt; margin: 0 0 7pt; padding-bottom: 4pt; border-bottom: 2px solid #333; page-break-after: avoid; }
h2 { font-size: 11pt; margin: 9pt 0 4pt; page-break-after: avoid; }
h3 { font-size: 9.5pt; margin: 7pt 0 3pt; page-break-after: avoid; }
p, li { margin: 0 0 3pt; }
code { font: 7.4pt 'SF Mono', Menlo, monospace; background: #f2f2f2; padding: 0 2px; border-radius: 2px; }
pre { font: 8.2pt/1.35 'SF Mono', Menlo, monospace; background: #f7f7f7; border-left: 2px solid #bbb;
      padding: 5pt 7pt; white-space: pre-wrap; word-break: break-word; page-break-inside: avoid; }
table { border-collapse: collapse; width: 100%; margin: 4pt 0; font-size: 7pt; }
th, td { border: 1px solid #ccc; padding: 2pt 3pt; text-align: left; vertical-align: top; word-break: break-word; }
th { background: #ececec; font-weight: 600; }
tr { page-break-inside: avoid; }
blockquote { margin: 6pt 0; padding-left: 8pt; border-left: 2px solid #999; color: #444; }
.seccion { page-break-before: auto; }
#portada { text-align: center; padding-top: 70mm; }
#portada h1 { border: 0; font-size: 24pt; }
#indice { page-break-before: always; }
#indice ol { font-size: 10.5pt; line-height: 1.9; }
figure { margin: 8pt 0; page-break-inside: avoid; text-align: center; }
figure img { max-width: 100%; max-height: 55mm; object-fit: contain; }
figcaption { font-size: 8.5pt; margin-top: 3pt; }
"""

def main():
    partes = [f'<div id="portada"><h1>Reto 2 · Patrones de diseño arquitectónico</h1>'
              f'<p style="font-size:12pt">Arquitectura de Software</p>'
              f'<p>Santiago HM · Simón BU · Sebastián QJ</p></div>']
    indice = ['<div class="seccion" id="indice"><h1>Índice</h1><ol>']
    cuerpo = []
    for n, (titulo, ruta) in enumerate(SECCIONES, 1):
        p = os.path.join(RAIZ, ruta)
        if not os.path.isfile(p):
            print(f'  falta: {ruta}', file=sys.stderr); continue
        md = open(p, encoding='utf-8').read()
        md = re.sub(r'^# .*\n', '', md, count=1)          # el H1 propio lo reemplaza el titulo de seccion
        indice.append(f'<li>{html.escape(titulo)}</li>')
        contenido = md_a_html(md, n)
        if ruta == '03-diseno/A3_DISENO_ASIS_TOBE.md':
            contenido += diagramas_uml()
        cuerpo.append(f'<div class="seccion" id="s{n}"><h1>{html.escape(titulo)}</h1>{contenido}</div>')
        print(f'  {n:2}. {titulo}')
    indice.append('</ol></div>')
    doc = (f'<!doctype html><meta charset="utf-8"><title>Reto 2</title><style>{CSS}</style>'
           + ''.join(partes + indice + cuerpo))
    open(SALIDA_HTML, 'w', encoding='utf-8').write(doc)

    subprocess.run([CHROME, '--headless', '--disable-gpu', '--no-pdf-header-footer',
                    f'--print-to-pdf={SALIDA_PDF}', '--virtual-time-budget=8000',
                    'file://' + SALIDA_HTML],
                   check=True, capture_output=True)
    print(f'\nPDF -> {SALIDA_PDF}')

    if LIBREOFFICE:
        with tempfile.TemporaryDirectory() as directorio_temporal:
            subprocess.run([LIBREOFFICE, '--headless', '--convert-to',
                            'docx:Office Open XML Text', '--outdir',
                            directorio_temporal, SALIDA_HTML],
                           check=True, capture_output=True)
            shutil.copyfile(
                os.path.join(directorio_temporal, 'documento.docx'), SALIDA_DOCX)
        print(f'DOCX -> {SALIDA_DOCX}')

if __name__ == '__main__':
    main()
