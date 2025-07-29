SUBDIRS = ./Sources 

all all-pdf all-docx clean \
startup-guide startup-guide-pdf startup-guide-docx \
set-up set-up-pdf set-up-docx \
: $(SUBDIRS)

$(SUBDIRS): FORCE
	make -C $@ $(MAKECMDGOALS)

FORCE:
