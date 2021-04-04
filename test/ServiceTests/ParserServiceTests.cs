using AutoFixture;
using LispParser.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispParser.Test.ServiceTests
{
    public class ParserServiceTests
    {
        private ParserService _sut;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _sut = new ParserService();
            fixture = new Fixture();
        }

        [Test]
        public void RemoveComments_ReturnsStringWithoutLispComments()
        {
            // Arrange
            var contents = GetTestContents();

            // Act
            var response = _sut.RemoveComments(contents);

            // Assert
            Assert.IsNotEmpty(response);
            Assert.IsTrue(!response.Contains(';'));
        }

        [Test]
        [TestCase("Test;")]
        [TestCase("Test;asdf")]
        [TestCase("Test;   ")]
        public void RemoveCommentFromRow_ReturnsRowWithoutComments(string row)
        {
            // Act
            var response = _sut.RemoveCommentFromRow(row);

            // Assert
            Assert.AreEqual("Test", response);
        }

        [Test]
        public void RemoveCommentFromRow_ReturnsOriginalRowWhenNoComment()
        {
            // Arrange
            var row = "Testing";

            // Act
            var response = _sut.RemoveCommentFromRow(row);

            // Assert
            Assert.AreEqual(row, response);
        }

        [Test]
        public void RemoveQuotes_ReturnsStringWithoutQuotes()
        {
            // Arrange
            var content = GetTestContents();

            // Act
            var response = _sut.RemoveQuotes(content);

            // Assert
            Assert.IsNotEmpty(response);
            Assert.IsTrue(!response.Contains("\""));
        }

        [Test]
        public void RemoveNextQuoteSet_ThrowsArgumentExceptionWhenNextQuoteNotFound()
        {
            // Arrange
            var content = "\"Test";
            var index = 1;
            var expectedMessage = "Number of quotes is uneven; file cannot be parsed.";

            // Act
            // Assert
            var response = Assert.Throws<ArgumentException>(() => _sut.RemoveNextQuoteSet(content, index));
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void RemoveNextQuoteSet_ReturnsStringOutsideOfQuotesWhenQuotesFound()
        {
            // Arrange
            var content = "Test\"Test\"Test";
            var index = content.IndexOf('\"');

            // Act
            var response = _sut.RemoveNextQuoteSet(content, index);

            // Assert
            Assert.AreEqual("TestTest", response);
        }

        [Test]
        [TestCase("(()))", 2, 3)]
        [TestCase("((())", 3, 2)]
        public void ValidateCount_ThrowsException_WhenRightAndLeftParenthesisUneven(string content, int leftParenthesisCount, int rightParenthesisCount)
        {
            // Arrange
            var expectedMessage = $"Found {leftParenthesisCount} '(' and {rightParenthesisCount} ')'.";

            // Act
            //Assert
            var response = Assert.Throws<ArgumentException>(() => _sut.ValidateCount(content));
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void ParseForParentheses_ReturnsOneHigherThanLeftCount_WhenLeftParenthesesCharacter()
        {
            // Arrange
            var character = '(';
            var initialCount = fixture.Create<int>();

            // Act
            var response = _sut.ParseForParentheses(character, initialCount);

            // Assert
            Assert.AreEqual(initialCount + 1, response);
        }

        [Test]
        public void ParseForParentheses_ReturnsOneLowerThanLeftCount_WhenLeftParenthesesCharacter()
        {
            // Arrange
            var character = ')';
            var leftCount = fixture.Create<int>();

            // Act
            var response = _sut.ParseForParentheses(character, leftCount);

            // Assert
            Assert.AreEqual(leftCount - 1, response);
        }

        [Test]
        public void HandleRightParentheses_ThrowsException_WhenLeftCountIs0()
        {
            // Arrange
            var leftCount = 0;
            var expectedMessage = "Improperly nested ')'.";

            // Act
            // Assert
            var response = Assert.Throws<ArgumentException>(() => _sut.HandleRightParentheses(leftCount));
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void HandleRightParentheses_ReturnsOneLowerThanLeftCount_WhenLeftCountIsGreaterThan0()
        {
            // Arrange
            var leftCount = fixture.Create<int>();

            // Act
            // Assert
            var response = _sut.HandleRightParentheses(leftCount);
            Assert.AreEqual(leftCount - 1, response);
        }

        private string GetTestContents()
        {
            return @"
;; Declare the correct package for this application; 
;; for this example, use the ""user"" package.
(in -package ""USER"")

; ; Define a default size for the queue.
(defconstant default - queue - size 100 ""Default size of a queue"")


                ; ; ; The following structure encapsulates a queue.  It contains a
                  ; ; ; simple vector to hold the elements and a pair of pointers to
                    ; ; ; index into the vector.  One is a ""put pointer"" that indicates
                      ; ; ; where the next element is stored into the queue.  The other is
                        ; ; ; a ""get pointer"" that indicates the place from which the next
                        ; ; ; element is retrieved.
  ; ; ;
            ; ; ; When put-ptr = get - ptr, the queue is empty.
                ; ; ; When put-ptr + 1 = get - ptr, the queue is full.
                    (defstruct(queue(:constructor create-queue)
                    
                                      (:print - function queue - print - function))
                    
                      (elements #() :type simple-vector)    
                                 ; simple vector of elements
  (put-ptr 0 :type fixnum)       ; next place to put an element
  (get-ptr 0 :type fixnum)       ; next place to take an element
  )


; ; To make QUEUE - NEXT efficient, give the Compiler some hints.
   (eval - when(compile eval)
     (proclaim '(inline queue-next))
     (proclaim '(function queue-next (queue fixnum) fixnum))
     )


   (defun queue - next(queue ptr)
   
     ""Increment a queue pointer by 1 and wrap around if needed.""
     (let((length(length(queue - elements queue)))
           (try (the fixnum(1 + ptr))))
    (if (= try length) 0 try)))


(defun queue - get(queue & optional(default nil))
  ; return DEFAULT if the queue is empty.""
  ""Get an element from QUEUE""
  (check - type queue queue)
  (let((get(queue - get - ptr queue))
        (put(queue - put - ptr queue)))
    (if (= get put)
        ; ; Queue is empty.
        default
        ; ; Get the element and update the get - ptr.
 
         (prog1
           (svref(queue - elements queue) get)
           (setf(queue - get - ptr queue)(queue - next queue get))))))


; ; Define a function to put an element into the queue.  If the
 ; ; queue is already full, QUEUE - PUT returns NIL.  If the queue
    ; ; isn't full, QUEUE-PUT stores the element and returns T.
     (defun queue - put(queue element)
     
       ""Store ELEMENT in the QUEUE and return T on success or NIL on failure.""
       (check - type queue queue)
       (let * ((get(queue - get - ptr queue))
              (put(queue - put - ptr queue))
              (next(queue - next queue put)))
         (unless(= get next)
     
           ; ; store element
            (setf (svref (queue-elements queue) put) element) 
      (setf(queue - put - ptr queue) next)      ; update put-ptr
      t)))                                   ; indicate success


; ; Define a SETF method.
(defsetf queue - get queue - put)


(defun queue - print - function(queue stream depth)
  ""This is the function used to print queue structures.""
  (declare(ignore depth))
  (multiple - value - bind(current - size max - size)
      (queue - length queue)
    (format stream ""#<Queue ~A/~A ~X>""
            current - size
            max - size
            (liquid::% pointer queue))))


(defun queue - length(queue)
  ""Returns as two values the number of elements in the queue 
   and the maximum number of elements the queue can hold.""
  (check - type queue queue)
  (let((length(length(queue - elements queue)))
        (delta(the fixnum(-(queue - put - ptr queue)
          (queue - get - ptr queue)))))
    (declare(fixnum length delta))
    ; ; The maximum number of elements the queue can hold is
 
     ; ; (1 - LENGTH) because a queue is empty when put - ptr =
   
       ; ; get - ptr.
    
        (values(mod delta length)(the fixnum(1 - length)))))


(defun queue - empty - p(queue)
  ""Return T if QUEUE is empty.""
  (check - type queue queue)
  (= (queue - put - ptr queue)(queue - get - ptr queue)))


(defun queue - full - p(queue)
  ""Return T if QUEUE is full.""
  (check - type queue queue)
  (= (queue - get - ptr queue)
     (queue - next queue(queue - put - ptr queue))))


; ; Create a queue.The :ELEMENTS keyword specifies a simple
; ; vector to hold the elements of the queue. Note that the
 ; ; maximum number of elements the queue can hold is one less than
  ; ; the length of the vector.
   (defun make - queue(&key(elements(make - array(1 + default - queue - size))))
   
     ""Create a queue.""
     (check - type elements simple - vector)
     (create - queue :elements elements))""";
        }
    }
}
