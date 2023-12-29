# GastrOS – Gastrointestinal Endoscopy Reporting Application

GastrOS is an endoscopic reporting application based on open standards: _open_EHR and MST. GUI is driven by Archetypes/Templates. It is part of our research at the University of Auckland to investigate software maintainability and interoperability.

**Features:**
* Uses openEHR Archetypes and Templates
* Based on the international standard terminology - MST
* Dynamic GUI creation and data binding
* Can create endoscopy reports
* Can be extended by modifying the model - no coding

The project consists of developing an openEHR based endoscopic reporting application using C# and MS .Net platform. The main goal is to deliver the first working prototype by March 2010 based on the very same requirements of an earlier application which has been developed, tested and validated as part of Koray Atalag’s Ph.D. research. This application is a ‘typical’ example of how most health information systems have been built so far: Object/Procedural (VB6) programming and a RDMS with hundreds of fields; a big ball of mud! Within the context of the wider research being undertaken by Dr. Atalag at the University of Auckland, Department of Computer Science, this project aims to help answer the following research question: **are we better off in terms of software maintainability using openEHR?** (hence a way of looking at the future-proof aspect). 

The content (i.e. terminology, record structure and semantics) will be depicted by the World Organisation of Digestive Endoscopy (OMED) official standard: the Minimal Standard Terminology for Digestive Endoscopy (MST). On the technical side, openEHR specifications shall be used for software development. The project is formally endorsed by both standards organisations and also funded by a research grant from the University of Auckland _(Project No: 3624469/9843, Open Standards Based Clinical Knowledge Modelling and Development of an Endoscopic Information System Project)_.

The main feature of this system will be the generation of dynamic graphical user interfaces from underlying domain knowledge model – MST Archetypes and Templates. Then it will explore ways of building the “value instance” informed by the underlying AOM and based on user data entered on the GUI. At the initial version data shall be persisted as simple XML files organised by folders by patient ID and loaded back into memory later on. It has been decided to utilise the available .Net ADL parser together with the generic XML serialiser which comes with the .Net framework. The approach taken is to utilise RM classes directly in openEhrV1 Assembly which contains the.Net C# implementation of the openEHR RM and some helper functions (to be FOSS’ed by Ocean Informatics).

The research involves formal measurement of software maintainability by using the framework brought about by the ISO/IEC 9216 and 25000 suite of Software Quality standards. One interesting feature of this research is that, because of the availability of two applications with same functionality, we will do a comparative assessment using both retrospective real change of requirements and also prospectively by determining new change requirements needed to run the application in a new setting. Other aspects include investigating the benefits of open systems vs. propriety systems and creating a clinically usable system which can then be exploited commercially.
