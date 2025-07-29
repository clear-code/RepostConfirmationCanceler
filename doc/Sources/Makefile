# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at http://mozilla.org/MPL/2.0/.

DATE=$(shell date +%Y%m%d)
BASE=$(shell pwd)
OUTDIR=$(BASE)/../Outs
# TOOLDIR=$(BASE)/../Tools

# How to generate a PDF document:
#
#   $ sudo apt install texlive-full pandoc fonts-noto-cjk
#   $ make (Name of Document)-pdf
#
PANDOC_OPT_PDF= -N --toc-depth=2 --table-of-contents \
                -f markdown+escaped_line_breaks \
                --wrap=preserve \
                -s --listings \
                --template=custom-eisvogel.latex \
                --lua-filter=line-break.lua \
                -V documentclass=ltjsarticle \
                -V classoption=titlepage \
                -V header-right=" " \
                -V table-use-row-colors=true \
                --pdf-engine=xelatex

PANDOC_OPT_DOCX= --toc-depth=2 --table-of-contents \
                 -f markdown+east_asian_line_breaks \
                 -t docx \
                 --reference-doc="$(BASE)/assets/style-reference.docx"

all: startup-guide set-up troubleshooting-guide distribute-in-house

all-pdf: startup-guide-pdf set-up-pdf troubleshooting-guide-pdf distribute-in-house-pdf

all-docx: startup-guide-docx set-up-docx troubleshooting-guide-docx distribute-in-house-docx

startup-guide: startup-guide-pdf startup-guide-docx

startup-guide-pdf:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)" 
	cat startup-guide.md | pandoc ${PANDOC_OPT_PDF} -o "$(OUTDIR)/startup-guide-$(DATE).pdf"

startup-guide-docx:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)"
	cat startup-guide.md | pandoc ${PANDOC_OPT_DOCX} -o "$(OUTDIR)/startup-guide-$(DATE).docx"

set-up: set-up-pdf set-up-docx

set-up-pdf:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)" 
	cat set-up.md | pandoc ${PANDOC_OPT_PDF} -o "$(OUTDIR)/set-up-$(DATE).pdf"

set-up-docx:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)"
	cat set-up.md | pandoc ${PANDOC_OPT_DOCX} -o "$(OUTDIR)/set-up-$(DATE).docx"

troubleshooting-guide: troubleshooting-guide-pdf troubleshooting-guide-docx

troubleshooting-guide-pdf:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)" 
	cat troubleshooting-guide.md | pandoc ${PANDOC_OPT_PDF} -o "$(OUTDIR)/troubleshooting-guide-$(DATE).pdf"

troubleshooting-guide-docx:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)"
	cat troubleshooting-guide.md | pandoc ${PANDOC_OPT_DOCX} -o "$(OUTDIR)/troubleshooting-guide-$(DATE).docx"

distribute-in-house: distribute-in-house-pdf distribute-in-house-docx

distribute-in-house-pdf:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)" 
	cat distribute-in-house.md | pandoc ${PANDOC_OPT_PDF} -o "$(OUTDIR)/distribute-in-house-$(DATE).pdf"

distribute-in-house-docx:
	test -d "$(OUTDIR)" || mkdir "$(OUTDIR)"
	cat distribute-in-house.md | pandoc ${PANDOC_OPT_DOCX} -o "$(OUTDIR)/distribute-in-house-$(DATE).docx"

clean:
	rm -rf $(OUTDIR)
