include config.make

SHARPEN=$(prefix)/sharpen.sh

JAVA_PATCH=$(prefix)/java.patch
CS_PATCH=$(prefix)/cs.patch

JGIT_DIR = source
JGIT_SOURCE = $(JGIT_DIR)/README
JGIT_GIT = @cd $(JGIT_DIR) && git

NGIT_DIR = generated
NGIT_GIT = @cd $(NGIT_DIR) && git

JSCH_SOURCE = $(JGIT_DIR)/jsch/README
JSCH_RELEASE = 0.1.43
JSCH_URL = http://downloads.sourceforge.net/project/jsch/jsch/$(JSCH_RELEASE)/jsch-$(JSCH_RELEASE).zip

JZLIB_SOURCE = $(JGIT_DIR)/jzlib
JZLIB_RELEASE = 1.0.7
JZLIB_URL = http://www.jcraft.com/jzlib/jzlib-$(JZLIB_RELEASE).zip

BIN_PATH=$(prefix)/bin

SB=\033[1m
EB=\033[m

$(JZLIB_SOURCE):
	@echo -e "$(SB)Getting JZlib sources$(EB)"
	wget $(JZLIB_URL)
	@echo -e "$(SB)Unpacking JZlib$(EB)"
	rm -rf jzlib-$(JZLIB_RELEASE)
	unzip jzlib-$(JZLIB_RELEASE).zip
	rm -rf jzlib-$(JZLIB_RELEASE).zip
	mv jzlib-$(JZLIB_RELEASE) $(JGIT_DIR)/jzlib
	mkdir -p $(JGIT_DIR)/jzlib/src
	mv $(JGIT_DIR)/jzlib/com $(JGIT_DIR)/jzlib/src
	@echo -e "$(SB)Building JZlib$(EB)"
	find $(JGIT_DIR)/jzlib/src -name *.java > resp
	mkdir -p $(BIN_PATH)
	javac @resp -d $(BIN_PATH)
	
$(JSCH_SOURCE):
	@echo -e "$(SB)Getting JSch sources$(EB)"
	wget $(JSCH_URL)
	@echo -e "$(SB)Unpacking JSch$(EB)"
	rm -rf jsch-$(JSCH_RELEASE)
	unzip jsch-$(JSCH_RELEASE).zip
	rm -rf jsch-$(JSCH_RELEASE).zip
	mkdir -p $(JGIT_DIR)/jsch
	mv jsch-$(JSCH_RELEASE)/* $(JGIT_DIR)/jsch
	rm -rf jsch-$(JSCH_RELEASE)
	@echo -e "$(SB)Building JSch$(EB)"
	find $(JGIT_DIR)/jsch/src -name *.java > resp
	mkdir -p $(BIN_PATH)
	javac @resp -d $(BIN_PATH) -cp $(BIN_PATH)

$(JGIT_SOURCE):
	@echo -e "$(SB)Getting jgit sources$(EB)"
	mkdir -p $(JGIT_DIR)
	git clone http://egit.eclipse.org/jgit.git $(JGIT_DIR)
	

# The steps for fetching are:
# - Fetch changes from git
# - Checkout the last version we generated
# - Generate all code for that last version. This code is the starting point for merging changes in the new code

fetch: $(JGIT_SOURCE) $(JZLIB_SOURCE) $(JSCH_SOURCE)
	@echo -e "$(SB)Pulling git sources$(EB)"
	$(JGIT_GIT) reset --hard
	$(JGIT_GIT) checkout master
	$(JGIT_GIT) pull origin
	@echo -e "$(SB)Applying previous java patch$(EB)"
	$(JGIT_GIT) checkout `cat ../lastrev`
	rm -rf $(NGIT_DIR)
	patch -s -d $(JGIT_DIR) -p1 < $(JAVA_PATCH)
	@echo -e "$(SB)Building jgit$(EB)"
	find $(JGIT_DIR)/org.eclipse.jgit/src -name *.java > resp
	javac @resp -d $(BIN_PATH) -cp $(BIN_PATH)
	@echo -e "$(SB)Building jgit.junit$(EB)"
	find $(JGIT_DIR)/org.eclipse.jgit.junit/src -name *.java > resp
	javac @resp -d $(BIN_PATH) -cp $(BIN_PATH):$(NUNIT_JAR)
	@echo -e "$(SB)Generating code for jzlib$(EB)"
	$(SHARPEN) $(JGIT_DIR)/jzlib jzlib src sharpen-options $(NGIT_DIR)/NSch jzlib.header
	@echo -e "$(SB)Generating code for jsch$(EB)"
	$(SHARPEN) $(JGIT_DIR)/jsch jsch src sharpen-options $(NGIT_DIR)/NSch jsch.header
	@echo -e "$(SB)Generating code for jgit (previous revision)$(EB)"
	$(SHARPEN) $(JGIT_DIR)/org.eclipse.jgit org.eclipse.jgit src sharpen-options $(NGIT_DIR)/NGit jgit.header
	@echo -e "$(SB)Generating code for jgit.nunit (previous revision)$(EB)"
	$(SHARPEN) $(JGIT_DIR)/org.eclipse.jgit.junit org.eclipse.jgit.junit src sharpen-options $(NGIT_DIR)/NGit.Test jgit.header
	@echo -e "$(SB)Generating code for jgit.test (previous revision)$(EB)"
	$(SHARPEN) $(JGIT_DIR)/org.eclipse.jgit.test org.eclipse.jgit.test tst sharpen-options $(NGIT_DIR)/NGit.Test jgit.header
	# Convert the generated code into git repos
	git init $(NGIT_DIR)
	$(NGIT_GIT) add . && git commit -a -m "Initial commit"
	@echo "Done."

merge:
	$(JGIT_GIT) reset --hard
	$(JGIT_GIT) checkout master
	@echo -e "$(SB)Building jgit$(EB)"
	find $(JGIT_DIR)/org.eclipse.jgit/src -name *.java > resp
	javac @resp -d $(BIN_PATH) -cp $(BIN_PATH)
	@echo -e "$(SB)Building jgit.junit$(EB)"
	find $(JGIT_DIR)/org.eclipse.jgit.junit/src -name *.java > resp
	javac @resp -d $(BIN_PATH) -cp $(BIN_PATH):$(NUNIT_JAR)
	@echo -e "$(SB)Applying java patch$(EB)"
	$(JGIT_GIT) branch -D merged 2> /dev/null || true
	$(JGIT_GIT) checkout -b merged `cat ../lastrev`
	patch -d $(JGIT_DIR) -p1 < $(JAVA_PATCH)
	$(JGIT_GIT) commit -a -m "Merge"
	@echo -e "$(SB)Merging new changes$(EB)"
	@echo -e "$(SB)In case of merge conflict, cd to the $(JGIT_DIR) directory, resolve the conflics and use 'git rebase --continue' to finish$(EB)"
	$(JGIT_GIT) rebase master

pull: fetch merge

validate-pull:
	# Generate a new patch
	$(JGIT_GIT) checkout merged
	$(JGIT_GIT) diff master > $(JAVA_PATCH)
	$(JGIT_GIT) reset --hard
	$(JGIT_GIT) checkout master
	$(JGIT_GIT) log -1 --format=format:%H master > ../lastrev

convert: validate-pull
	@echo -e "$(SB)Prepare git directories$(EB)"
	$(NGIT_GIT) reset --hard
	$(JGIT_GIT) checkout merged
	rm -rf source/org.eclipse.jgit.test/tst/org/eclipse/jgit/nls/RootLocaleTest.java
	$(NGIT_GIT) reset --hard
	$(NGIT_GIT) checkout master
	$(NGIT_GIT) branch -D last 2> /dev/null || true
	$(NGIT_GIT) checkout -b last
	@echo -e "$(SB)Generating code for jzlib$(EB)"
	$(SHARPEN) $(JGIT_DIR)/jzlib jzlib src sharpen-options $(NGIT_DIR)/NSch jzlib.header
	@echo -e "$(SB)Generating code for jsch$(EB)"
	$(SHARPEN) $(JGIT_DIR)/jsch jsch src sharpen-options $(NGIT_DIR)/NSch jsch.header
	@echo -e "$(SB)Generating code for jgit$(EB)"
	$(SHARPEN) $(JGIT_DIR)/org.eclipse.jgit org.eclipse.jgit src sharpen-options $(NGIT_DIR)/NGit jgit.header
	@echo -e "$(SB)Generating code for jgit.jnunit$(EB)"
	$(SHARPEN) $(JGIT_DIR)/org.eclipse.jgit.junit org.eclipse.jgit.junit src sharpen-options $(NGIT_DIR)/NGit.Test jgit.header
	@echo -e "$(SB)Generating code for jgit.test$(EB)"
	$(SHARPEN) $(JGIT_DIR)/org.eclipse.jgit.test org.eclipse.jgit.test tst sharpen-options $(NGIT_DIR)/NGit.Test jgit.header
	$(NGIT_GIT) add .
	$(NGIT_GIT) commit -a -m "New code" || true
	@echo -e "$(SB)Applying old patch$(EB)"
	$(NGIT_GIT) branch -D merged 2> /dev/null || true
	$(NGIT_GIT) checkout -b merged master
	patch -d $(NGIT_DIR) -p1 < $(CS_PATCH)
	$(NGIT_GIT) commit -a -m "Merge"
	@echo -e "$(SB)Merging new changes$(EB)"
	@echo -e "$(SB)In case of merge conflict, cd to the $(NGIT_DIR) directory, resolve the conflics and use 'git rebase --continue' to finish$(EB)"
	$(NGIT_GIT) rebase last

validate-convert:
	# update the patch
	$(NGIT_GIT) checkout merged
	$(NGIT_GIT) diff last > $(CS_PATCH)
	$(NGIT_GIT) reset --hard
	# update the master branch to last
	$(NGIT_GIT) checkout last
	$(NGIT_GIT) branch -D master 2> /dev/null || true
	$(NGIT_GIT) checkout -b master
	$(NGIT_GIT) checkout merged

import: validate-convert
	cp -r $(NGIT_DIR)/* ..
	rm -rf ../NGit/.git
	rm -rf ../NGit.Test/.git
	rm -rf ../NGit.NSch/.git

update-source:
	$(NGIT_GIT) checkout merged
	cd ../NGit && find . -type f -name '*.cs' -exec cp {} $(prefix)/generated/NGit/{} \;
	cd ../NGit.Test && find . -type f -name '*.cs' -exec cp {} $(prefix)/generated/NGit.Test/{} \;
	cd ../NSch && find . -type f -name '*.cs' \! -path '*Sharpen*' -exec cp {} $(prefix)/generated/NSch/{} \;
	
update-patch: update-source validate-convert


commit:
	git show master:gen/lastrev > prevrev
	echo "Updated from JGit" > msg
	$(JGIT_GIT) log "--format=format:----------------------------------------------------%nJGit commit %H%n%B" `cat $(prefix)/prevrev`..`cat $(prefix)/lastrev` >> $(prefix)/msg
	git commit -a -F msg

