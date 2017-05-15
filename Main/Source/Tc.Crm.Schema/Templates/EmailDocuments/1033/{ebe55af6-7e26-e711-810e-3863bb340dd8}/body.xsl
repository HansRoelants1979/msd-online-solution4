<?xml version="1.0" ?><xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"><xsl:output method="text" indent="no"/><xsl:template match="/data"><![CDATA[<p><font face="Tahoma, Verdana, Arial" size=2 style="display:inline;"><span style="font-family:&quot;Times New Roman&quot;,serif;font-size:12pt;">Dear&#160;]]><xsl:choose><xsl:when test="contact/fullname"><xsl:value-of select="contact/fullname" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ <br>
<br>
Customer lead booking name:&#160;&#160;]]><xsl:choose><xsl:when test="contact/fullname"><xsl:value-of select="contact/fullname" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ <br>
Booking reference:&#160; ]]><xsl:choose><xsl:when test="incident/tc_bookingreference/@name"><xsl:value-of select="incident/tc_bookingreference/@name" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ <br>
<br>
Following our conversations we've now completed our investigations.<br>
We're really sorry about the issues you had during your stay with us and want
to confirm that we've offered you and your party the e-Voucher below as full
and final settlement for the feedback you gave us.<br>
Thanks for giving us the chance to put things right. &#160;We hope we're able
to look after you on holiday with us again soon.</span></font></p><p><br></p><font face="Tahoma, Verdana, Arial" size=2 style="display:inline;"><p><br></p><p style="margin:0cm 0cm 0pt;"><span style="background:white;font-family:&quot;Tahoma&quot;,sans-serif;">Kind regards,</span></p><p style="margin:0cm 0cm 0pt;"><span style="background:white;font-family:&quot;Tahoma&quot;,sans-serif;"><br></span></p><p style="margin:0cm 0cm 0pt;"><span style="background:white;font-family:&quot;Tahoma&quot;,sans-serif;"><br></span></p><p style="margin:0cm 0cm 0pt;"><span style="background:white;font-family:&quot;Tahoma&quot;,sans-serif;"><img></span></p><p style="margin:0cm 0cm 0pt;"><span style="background:white;font-family:&quot;Tahoma&quot;,sans-serif;"><br></span></p><p style="background:white;margin:3.75pt 0cm;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;">This correspondence is confidential, may be legally privileged and is for the intended recipient only. Access, disclosure, copying, distribution or reliance on any of it by anyone else is prohibited and may be a criminal offence. Please delete if obtained in error.</span></p><p><br></p><p style="background:white;margin:3.75pt 0cm;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;">Any views expressed in this message are those of the individual sender, except where the sender specifically states them to be otherwise.</span></p><span style="font-family:&quot;Times New Roman&quot;,serif;font-size:12pt;"><p>

<br></p><div align=center><table width="100%" style="width:100%;border-collapse:collapse;" border=0 cellspacing=0 cellpadding=0>
 <tbody><tr style="">
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Name:</span></strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">&#160;&#160;&#160;]]><xsl:choose><xsl:when test="contact/fullname"><xsl:value-of select="contact/fullname" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; </span></p></td>
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your old booking reference:&#160;</span></strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">&#160;&#160;&#160; 
 ]]><xsl:choose><xsl:when test="incident/tc_bookingreference/@name"><xsl:value-of select="incident/tc_bookingreference/@name" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ </span></p><font size=3>
  </font></td>
 </tr>
 <tr style="">
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Expiry Date: &#160; Sat, 17 Feb 2018 </span></strong></p><font size=3>
  </font></td>
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your new booking reference:</span></strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">&#160;&#160;</span><span style="color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;"> </span></p><font size=3>
  </font></td>
 </tr>
 <tr style="">
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Value: &#160;</span></strong><span style="color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;">&#160;50.00</span></p><font size=3>
  </font></td>
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your Case Reference:</span></strong><span style="color:rgb(51, 51, 51);font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; &#160;]]><xsl:choose><xsl:when test="incident/ticketnumber"><xsl:value-of select="incident/ticketnumber" /></xsl:when><xsl:otherwise></xsl:otherwise></xsl:choose><![CDATA[ </span></p><font size=3>
  </font></td>
 </tr>
 <tr style="">
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><span style="color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-size:10pt;">&#160;</span></p><font size=3>
  </font></td>
  <td style="padding:3.75pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><span style="color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-size:10pt;">&#160;</span></p><font size=3>
  </font></td>
 </tr>
</tbody></table>

</div><p><span style="font-family:&quot;Tahoma&quot;,sans-serif;"><font size=5>Using
your voucher</font></span></p><p>

<br></p><div align=center>

<table width="100%" style="width:100%;border-collapse:collapse;" border=0 cellspacing=0 cellpadding=0>
 <tbody><tr style="">
  <td style="padding:7.5pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><ul><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Courier New&quot;;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">You can book online at&#160;</span><span style="color:windowtext;"><a href="http://www.thomascook.com/" target=_blank><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;"><font color="#0088cc">www.thomascook.com</font></span></a></span><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">,&#160;</span><span style="color:windowtext;"><a href="http://cp.mcafee.com/d/5fHCN0e40UedEIf3Cm4nxNEVdTdEEIIenjopsdEEIIenjoud79J54sOYejss76QkhP1EVpopohdNHiDaOfxUaCjGHJblGlVsTcDlnqmHkHOVJNAQsT7qY_R-u7ffITWZOWrbXT8K6zBCWbTkhpmKCHt5DBgY-F6lK1FJ4SCrLOtT61MUsOCYrKr01zQXBeCjGHJblGlSkjVg-DbCVAWGXiRqBiUJFvU6nM0LzArELw1zmBelAv3PFAWGXiRqBun1l5EOwhVnpQ9RFfHlVIGrsususdxCHtyvNd43IqGCy04AWGXgQg2lqBg3d40Bllo_pgCq80nXLN-5Liid41Frdbo94yZXELTppdbFFtdK9CQ2lfynXcW_43" target=_blank><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;"><font color="#0088cc">www.thomascookairlines.com</font></span></a></span><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">, over the phone, or in one of our UK travel agency
       stores. &#160;You can also book with any&#160;ABTA bonded UK travel
       agent authorised to sell holidays organised by Thomas Cook Tour
       Operations Limited. &#160;You'll need to pay either a deposit or the
       full balance as requested by the website/your travel agent, depending on
       your departure date.</span><span style="font-family:&quot;Courier New&quot;;"> </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Courier New&quot;;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">You&#160;can redeem your voucher against a package
       holiday operated by Thomas Cook Tour Operations Limited under any of the
       following brand names:&#160;<strong><span style="font-family:&quot;Tahoma&quot;,sans-serif;">Thomas
       Cook, Airtours, Thomas Cook City Escapes,Club 18-30, Cresta, Thomas Cook
       Signature, Flexibletrips.</span></strong>&#160;&#160;You can also redeem
       it against a flight booking with Thomas Cook Airlines.</span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Courier New&quot;;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">When your new booking is confirmed, either you or
       your travel agent can redeem your voucher by e-mail or by post.
       &#160;Simply e-mail us at&#160;</span><span style="color:windowtext;"><a href="mailto:voucher.redemptions@thomascook.com"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;"><font color="#0088cc">voucher.redemptions@thomascook.com</font></span></a></span><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">&#160;and tell us the voucher amount, the name on the
       voucher and the original and new booking references.&#160;</span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Courier New&quot;;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">By post, add your new booking reference to your
       voucher and send it to Customer Relations Department, Westpoint,
       Peterborough Business Park, Lynch Wood, Peterborough, PE2 6FZ.</span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Courier New&quot;;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">When we receive your email/voucher, we'll either
       apply a credit to your holiday booking and this will show on your new
       invoice that we send you or we'll refund you up to the value of the
       voucher.&#160; This can take up to 28 days to process.&#160;</span></li><font size=3>
  </font></ul><font size=3>
  </font><p style="margin:0cm 0cm 0pt;"><span style="color:rgb(51, 51, 51);font-family:&quot;Courier New&quot;;"><font size=3>&#160;</font></span></p><font size=3>
  </font></td>
 </tr>
</tbody></table>

</div><p>

<br></p><h3 style="margin:0cm 0cm 6pt;"><font size=5><span style="">&#160; </span><span style="font-family:&quot;Tahoma&quot;,sans-serif;">There
are some terms and conditions</span><span style="">
</span></font></h3><p>

<br></p><div align=center>

<table width="100%" style="width:100%;border-collapse:collapse;" border=0 cellspacing=0 cellpadding=0>
 <tbody><tr style="">
  <td style="padding:7.5pt;border:rgb(0, 0, 0);background-color:transparent;"><font size=3>
  </font><ul><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your voucher has to be used as part payment or full
       payment (depending on the value of it) against one new or existing
       booking. It cannot be applied to multiple bookings.</span><span style="font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;"> </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">A refund cannot be given if the value of your new
       holiday is less than the value of your voucher.Your voucher cannot be
       exchanged for cash and will automatically become invalid if it's not
       used on a new booking by the expiry date shown.</span><span style="font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;"> </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your voucher has to be used towards a new or existing
       booking by the expiry date shown and no extensions are allowed but your
       travel date can still be after the expiry date on the voucher.</span><span style="font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;"> </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your voucher is not transferable and has to be used
       by a person on the original booking.</span><span style="font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;">
       </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Your voucher cannot be used against any brand we
       haven't listed above (including but not limited to:- Thomas Cook Sport )
       or towards payment for any foreign exchange transaction, or for any
       travel arrangements organised, operated or supplied by a company that
       isn't part of the Thomas Cook Group, even if these arrangements are
       booked via a Thomas Cook or The Co-operative Travel store.&#160;</span><span style="font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;"> </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">Incorrect use of your voucher can result in
       cancellation charges being applied for any booking made in error, or
       full payment being required by another means.</span><span style="font-family:&quot;Helvetica&quot;,sans-serif;font-size:9pt;"> </span></li><font size=3>
   </font><li style="margin:0cm 0cm 0pt;color:rgb(51, 51, 51);font-family:&quot;Helvetica&quot;,sans-serif;font-style:normal;font-weight:normal;"><span style="font-family:&quot;Tahoma&quot;,sans-serif;font-size:9pt;">All holidays are subject to availability and the tour
       operator booking conditions will still apply to any holiday booked using
       this voucher</span></li><font size=3>
  </font></ul><font size=3>
  </font></td>
 </tr>
</tbody></table>

</div><p>

<br></p></span><p><br></p></font><p><br></p>]]></xsl:template></xsl:stylesheet>