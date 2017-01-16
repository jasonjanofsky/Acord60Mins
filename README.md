# Acord60Mins
Build an Acord Life Insurance Standard Integration in 60 Minutes!

The Acord in 60 Minutes Projects is a sample that can help creating an Acord for Life Insurance integration simple and robust.  There are four objectives to this project.

1. Serialize and Deserialize Acord XML from string to easy to digest objects.
2. Allow simple conversion of Acord files from Acord -> slightly different Acord file or completely different file -> Acord.
3. Provide a validator to determine if an XML file adheres to an Acord standard file.

As of this writing the latest Acord standard is 2.36, (1/16/2017).

There are 5 different projects within the Acord60Mins solution, they are:
1. AcordToolkit - contains the XSD.exe rendering of the schema file TXLife2.36.00.xsd.  In the project, there is a notes file named "XSDEXENotes.txt" that contains the exact command prompt string that was used to create the class file "txlife2_36_00.cs".

Note: If regenerating the class definition with XSD.exe, there is one property that needs to be commented out or serialization will not work properly.  The object is "ComboRelationship_Type".

2. Acord60Mins - contains an IncomingRequest object that can be used to wire up into a web method or other reception engine.  This file can/should be altered for your use and customized to receive files into your system.  A reflection helper class is included in here as well to allow the converters to run.

3. ConverterRules - While one can certainly write an Acord serializer/deserializer with a fair amount of ease, in a real world scenario, multiple integrations usually come with multiple interpretations of the Acord standard.  In these situations, one must convert other interpretations into one's own format.  For this situation, this project uses a library (Converters) to store code for different client conversions.  The ConverterRules project defines a rules for an XML file that is setup to declare what conversion should be used for which client.

4. Converters - As the above project ConverterRules notes, this project stores the conversion classes for different possible clients.  The interface IConverter simply let's one know how to implement a converter.  From there the converter can be wired up using the ConverterRules.xml.

5. AcordTest - The test project for the solution.  This project is the key to understanding how the solution works as it also serves as a harness to call the main methods of the application.  To understand how everything works, just debug and step through the code in the tests.
