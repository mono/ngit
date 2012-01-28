# If a valid eclipse cannot be found, assume macos and that it's
# in /Applications
eclipse=`which eclipse`
if [ -z $eclipse ] ; then
	eclipse=/Applications/eclipse/eclipse
else
	eclipse=`tail -n1 $eclipse | cut -f2 -d " "`
fi

if ! `test -e $eclipse` ; then
        echo Eclipse could not be detected $eclipse
        exit 1
fi

# now that eclipse has been detected, export our junk
ECLIPSE_PATH=`dirname $eclipse`
NUNIT_JAR=`ls $ECLIPSE_PATH/plugins/org.junit*/*.jar | tail -n1`
HAMCREST_JAR=`ls $ECLIPSE_PATH/plugins/org.hamcrest*.jar | tail -n1`
BIN_PATH=`pwd`/bin

sed -e s:@NUNIT_JAR@:$NUNIT_JAR: -e s:@HAMCREST_JAR@:$HAMCREST_JAR: -e s:@BIN_PATH@:$BIN_PATH: < sharpen-options.in > sharpen-options
echo prefix=`pwd` > config.make
echo ECLIPSE_PATH=$ECLIPSE_PATH >> config.make
echo NUNIT_JAR=$NUNIT_JAR >> config.make
echo Eclipse path: $ECLIPSE_PATH

