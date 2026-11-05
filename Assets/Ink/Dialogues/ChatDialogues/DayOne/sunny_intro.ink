VAR nextChatArea = ""
VAR nextBranch = ""

# speaker: Player
# portrait: player_portrait
+[hi, is this 12_sunny from Reddnet]

- # speaker: Sunny
# portrait: sunny_portrait
hey, heyy!
yep, it's me

# speaker: Player
# portrait: player_portrait
+[so...]
- i didn't know i wasn't alone

# speaker: Sunny
# portrait: sunny_portrait
of course not!
well... especially the ones who actually care about what they're doing
didn't you say you always feel weird when people praise you?
like... you know you did the work, but it still feels like you somehow don't deserve it? 

# speaker: Player
# portrait: player_portrait
+[yea, every time i get praised, i don't feel like i deserve it]
- Like they're talking about someone else
I'm not that great

# speaker: Sunny
# portrait: sunny_portrait
yeah, that's how it felt for me too
it's like your brain refuses to accept any of it even tho you know you did all that

# speaker: Player
# portrait: player_portrait
+[it's exhausting]

- # speaker: Sunny
# portrait: sunny_portrait
i went through that for a long time 
trapped between “I’m not enough” and “I can’t stop now”
and because everyone sees you as "good," you kinda force yourself to keep going
eventually it feels normal, even tho it isn't

# speaker: Player
# portrait: player_portrait
+[you talk like someone who's figured it out] -> branch1
+[so what changes for you?] -> branch2

==branch1==
# speaker: Sunny
# portrait: sunny_portrait
i would't say "figured out"
but i stopped trying to fix myself with the wrong things
i tried therapy, didn't help
those self-help books make it worse for me
then i met these people online

-> continue

==branch2==
# speaker: Sunny
# portrait: sunny_portrait
well...
i met these people online

-> continue

==continue==
# speaker: Sunny
# portrait: sunny_portrait
people who felt the same
we just talked
but everything just feels better
they help me see how much that self doubt wasn't even mine
it was a sign i've been listening to echoes from the dark for far too long 

# speaker: Player
# portrait: player_portrait
+[echoes from the dark? you mean like the people around you?]
+[what do you mean?]

- # speaker: Sunny
# portrait: sunny_portrait
it's hard to explain until you've seen it yourself
you'll realize the echoes
the lies people live by just to get through the day
you'd be surprised how free it is once you see it
we lift each other up by listening
reminding one another that every sunset is just the promise of another dawn

# speaker: Player
# portrait: player_portrait
+[so... it's an echoes server?]
+[is it like a support group?]

- # speaker: Sunny
# portrait: sunny_portrait
yeah, it started as a study group but turned into something more like small community
there's someone that started this group, his name is Rael
he's... different
he helped us a lot
the one that shows us that light never truly leaves
we usually meet at night since everyone have their own thing in the day
the server is invite only
i can send you the link if you want, no pressure tho

# speaker: Player
# portrait: player_portrait
+[sure, can't be worse than doomscrolling] -> branch3
+[invite only huh...] -> branch4

==branch3==
# speaker: Sunny
# portrait: sunny_portrait
i assure you, it'll be worth your time
i'm sure that they'll help you as much as they helped me

->continue2

==branch4==
# speaker: Sunny
# portrait: sunny_portrait
i know it's weird to trust strangers right away
but no one's there gonna pressure you to do anything
you can look around first
still... i think you'd like it

->continue2

==continue2==
- # speaker: Sunny
# portrait: sunny_portrait
~ nextChatArea = "ChatAreaSunny"
~ nextBranch = "sunny_intro2"
i'll sent you the link later tonight once i got the permission
just think about it first

-> END