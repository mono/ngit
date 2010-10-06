ECLIPSE_SCRIPT=`which eclipse`
ECLIPSE_PATH=`tail -n1 $ECLIPSE_SCRIPT | cut -f2 -d " "`
ECLIPSE_PATH=`dirname $ECLIPSE_PATH`
NUNIT_JAR=`ls $ECLIPSE_PATH/plugins/org.junit*/*.jar | tail -n1`
BIN_PATH=`pwd`/bin

sed -e s:@NUNIT_JAR@:$NUNIT_JAR: -e s:@BIN_PATH@:$BIN_PATH: < sharpen-options.in > sharpen-options
echo prefix=`pwd` > config.make
echo ECLIPSE_PATH=$ECLIPSE_PATH >> config.make
echo NUNIT_JAR=$NUNIT_JAR >> config.make
echo Eclipse path: $ECLIPSE_PATH

