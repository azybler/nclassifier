using NClassifier;
using NClassifier.Summarizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NClassifierTestProgram
{
   class Program
   {
      static void Main(string[] args)
      {
         string result;
         string[] sentences;

         string input = @"
I've become a huge fan of touch computing. I believe that most things we think of as 'computers' will be de-facto tablets, either in our pocket, in our hands, possibly even mounted on our wrists or forearms.
I can't wait for the iPad 5 this week (I'll be ordering three), and my Surface Pro 2 should arrive this week too. Because it is a blazingly fast, modern Intel machine, I like to use the Surface Pro to predict where tablet performance ought to be for everyone in 2 to 3 years. I think of it as an iPad 7.
My main complaint with the Surface Pro is the incredibly lackluster battery life. Granted, this is a classic Intel x86 box we're talking about, not some efficient ARM system-on-a-chip designed to run on a tiny battery. Still, I was hopeful that the first Surface Pro with Haswell inside would produce giant gains in battery life as Intel promised. Then I saw this graph:
So WiFi web browsing battery life, arguably the most common user activity there is on a computer these days, goes from 4.7 hours on the Surface Pro to 6.7 hours on the Surface Pro 2, a 42% increase. That's a decent increase, I suppose, but I was hoping for something more like 8 hours, something closer to doubling of battery life – to bring the Surface Pro in line with other tablets.
Nearly 7 whole hours of WiFi web browsing for a real computer in tablet form factor … that's not bad, right? Let's see how the 2013 MacBook Air does, which spec-wise is about as close as we can get to the Surface Pro 2. The screen is somewhat lower resolution and not touch capable, of course, but under the hood, the i5-4200u CPU and LPDDR3 RAM are nearly the same. It's a real computer, too, using the latest Intel technology.
The Surface Pro 2 has a 42 Wh battery, which puts it closer to the 11 inch Air in capacity. Still, over 11 hours of battery life browsing the web on WiFi? That means the Air is somehow producing nearly two times the battery efficiency of the best hardware and software combination Microsoft can muster, for what I consider to be the most common usage pattern on a computer today. That's shocking. Scandalous, even.
UPDATE: Turns out the Surface 2 Pro was shipped with bad firmware. Once updated, the WiFi adapter enters lower idle power states and this helps a lot, going from 6.6 hours of browsing time to 8.3 hours, a 25% improvement! That puts it much more in line with the rest of the field, at least, even if it doesn't achieve Mac like runtime.
It's not exactly news that Windows historically doesn't do as well as OS X on battery life. Way back in 2009, AnandTech tested a MacBook Pro with multiple operating systems.
2009 15-inch MacBook Pro (73WHr battery)	OS X 10.5.7	Windows Vista x64 SP1	Windows 7 RC1.
Wireless Web Browsing (No Flash) Battery Life	8.13 hours	6.02 hours	5.48 hours.
That's fine, I knew about this discrepancy, but here's what really bothers me:
The Windows light usage battery life situation has not improved at all since 2009. If anything the disparity between OS X and Windows light usage battery life has gotten worse.
Microsoft positions Windows 8 as an operating system that's great for tablets, which are designed for casual web browsing and light app use – but how can that possibly be true when Windows idle power management is so much worse than the competition's desktop operating system in OS X – much less their tablet and phone operating system, iOS?
(It's true that Bay Trail, Intel's new lower power CPU from the Atom family, achieves 8.6 hours of WiFi web browsing. That's solidly in the middle of the tablet pack for battery life. But all the evidence tells me that the very same hardware would do a lot better in OS X, or even iOS. At least Intel has finally produced something that's reasonably competitive with the latest ARM chips.)
Perhaps most damning of all, if you take the latest and greatest 13 inch MacBook Air, and install Windows 8 on it, guess what happens to battery life?
One of the best things about the standard 2013 MacBook Air 13 inch is that it has record-breaking battery life of 14 hrs 25 min (with the screen brightness at 100 cd/m², headphones plugged in and the Wi-Fi, Bluetooth and keyboard backlighting turned off). Under Windows 8 the results are more mixed [..] in the same conditions it lasts only 7 hrs 40 min. That's still very high—it's better than the Asus Zenbook Prime UX31A's 6 hours and the Samsung Series 7 Ultra's 5 hours—but it's only half the astronomical 14 hours + that the 13 inch MacBook Air is capable of.
Instead of the 26% less battery life in Windows that Anand measured in 2009, we're now seeing 50% less battery life. This is an enormous gap between Windows and OS X in what is arguably the most common form of computer usage today, basic WiFi web browsing. That's shameful. Embarrassing, even.
I had a brief Twitter conversation with Anand Shimpi of Anandtech about this, and he was as perplexed as I was. Nobody could explain the technical basis for this vast difference in idle power management on the same hardware. None of the PC vendors he spoke to could justify it, or produce a Windows box that managed similar battery life to OS X. And that battery life gap is worse today – even when using Microsoft's own hardware, designed in Microsoft's labs, running Microsoft's latest operating system released this week. Microsoft can no longer hand wave this vast difference away based on vague references to 'poorly optimized third party drivers'.
Apple is clearly doing a great job here. Kudos. If you want a device that delivers maximum battery life for light web browsing, there's no question that you should get something with an Apple logo on it. I just wish somebody could explain to me and Anand why Windows is so awful at managing idle power. We're at a loss to understand why Windows' terrible – and worsening! – idle battery life performance isn't the source of far more industry outrage.
         ";
         input = input.Trim();

         SimpleSummarizer simpleSummarizer = new SimpleSummarizer();
         result = simpleSummarizer.Summarize(input, 5);
         sentences = Utilities.GetSentences(result);
         foreach (string sentence in sentences)
         {
            Console.WriteLine(sentence);
            Console.WriteLine();
         }

         Console.WriteLine("=========================");

         SimonSummarizer simonSummarizer = new SimonSummarizer();
         result = simonSummarizer.Summarize(input, 5);
         sentences = Utilities.GetSentences(result);
         foreach (string sentence in sentences)
         {
            Console.WriteLine(sentence);
            Console.WriteLine();
         }

         Console.ReadLine();
      }
   }
}
